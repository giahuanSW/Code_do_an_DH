#include <TimerOne.h>
#include <Keypad.h>
#include <LiquidCrystal_I2C.h>
LiquidCrystal_I2C lcd(0x27,16,2);
const byte ROWS = 4;
const byte COLS = 3;
char keys[ROWS][COLS] = {
  {'1','2','3'},
  {'4','5','6'},
  {'7','8','9'},
  {'*','0','#'}
};
byte rowPins[ROWS] = {13, 12, 11, 10}; 
byte colPins[COLS] = {9,8,7};
char temp[255];
int i = 0, dem = 0; 
Keypad keypad = Keypad( makeKeymap(keys), rowPins, colPins, ROWS, COLS );
double T = 0.01, T1 = 0.2, xung, vong;
double vitri, Vitridat, tocdo, Tocdodat;
double E , E1, E2, E3, E4, E5;
double alpha, beta, gamma;
double Kp, Ki, Kd;
double Output, LastOutput, Output_1, LastOutput_1;
String inString = "";
int mode, active, stop_st, temp1, temp2, temp3, temp4;
int status_st = 0;
void setup() {
  // put your setup code here, to run once:
    
    pinMode(2,INPUT_PULLUP);    //chan ngat
    pinMode(4,INPUT_PULLUP);    //chan doc encoder
    pinMode(3,OUTPUT);
    pinMode(5,OUTPUT);
    pinMode(6,OUTPUT);
    lcd.init();
    lcd.backlight();
    Serial.begin(9600);
    analogWrite(3,0);
    digitalWrite(5,LOW);
    digitalWrite(6,LOW);
    while(Serial.available() == 0){

    }
    while(Serial.available() > 0){
    inString = Serial.readStringUntil('\n');
    mode = inString.toFloat();
    inString = Serial.readStringUntil('\n');
    active = inString.toFloat();
    inString = Serial.readStringUntil('\n');
    temp1 = inString.toFloat();
    inString = Serial.readStringUntil('\n');
    temp2 = inString.toFloat();
    inString = Serial.readStringUntil('\n');
    temp3 = inString.toFloat();
    inString = Serial.readStringUntil('\n');
    temp4 = inString.toFloat();  
    inString = "";
}
if(mode == 0){
  if(active == 0)
    Vitridat = temp1;
  else
    Tocdodat = temp1;
    Kp = temp2;
    Ki = temp3;
    Kd = temp4;
}
if(mode == 1){
  getkey();
}
    if(active == 0)
    {  
    attachInterrupt(0,Demxung, FALLING);
    Timer1.initialize(10000);     //us 
    Timer1.attachInterrupt(PID_CAL);    
    }
    if(active == 1)
    { 
    attachInterrupt(0,Demxung, FALLING);
    Timer1.initialize(200000);     //us 
    Timer1.attachInterrupt(PID_CAL_1);      
    }
} 
void loop(){
if(active == 0)
  delay(10);
if(active == 1)
  delay(200);
if(status_st == 1)
Serial.print("forward");
else
Serial.print("reverse");
Serial.print("|");
Serial.print(vitri);
Serial.print("|");
Serial.println(tocdo);
if(Serial.available()){
    inString = Serial.readStringUntil('\n');
    stop_st = inString.toInt();
    inString = "";
    if(stop_st == 1){
      detachInterrupt(0);
      Timer1.detachInterrupt();
      asm volatile ( "jmp 0");
  }
  }
}
void Demxung()
{
    if(digitalRead(4) == LOW)
    {
    xung++;
    status_st = 1;
    }
    else{
    xung--;
    status_st = 0;
    }
}
void PID_CAL()
{
  vitri = (xung*360)/333;
  E = Vitridat - vitri;
  alpha = 2*T*Kp + Ki*T*T + 2*Kd;
  beta = T*T*Ki - 4*Kd - 2*T*Kp;
  gamma = 2*Kd;
  Output = (alpha*E +beta*E1 + gamma*E2 +2*T*LastOutput)/(2*T);
  LastOutput = Output;
  E2 = E1;
  E1 = E;
  if(Output > 255)
  Output = 255;
  if(Output < -255)
  Output = -255;
  if(Output > 0)
  {
    analogWrite(3,Output);
    digitalWrite(5,HIGH);
    digitalWrite(6,LOW);
  }
  else if(Output < 0)
  {
    analogWrite(3,abs(Output));
    digitalWrite(5,LOW);
    digitalWrite(6,HIGH);
  }
  else
  {
    analogWrite(3,0);
    digitalWrite(5,LOW);
    digitalWrite(6,LOW);
  }
}
void PID_CAL_1()
{
  tocdo = (xung/333)*(1/T1)*60;
  xung = 0;
  E3 = Tocdodat - tocdo;
  alpha = 2*T1*Kp + Ki*T1*T1 + 2*Kd;
  beta = T1*T1*Ki - 4*Kd - 2*T1*Kp;
  gamma = 2*Kd;
  Output_1 = (alpha*E3 +beta*E4 + gamma*E5 +2*T1*LastOutput_1)/(2*T1);
  LastOutput_1 = Output_1;
  E5 = E4;
  E4 = E3;
  if(Output_1 > 255)
  Output_1 = 255;
  if(Output_1 < 0)
  Output_1 = 0;
  if(Output_1 > 0)
  {
    analogWrite(3,Output_1);
    digitalWrite(5,HIGH);
    digitalWrite(6,LOW);
  }
  else
  {
    analogWrite(3,0);
    digitalWrite(5,LOW);
    digitalWrite(6,LOW);
  }
}
void getkey(){
  while(dem<4){
  char key = keypad.getKey();
  delay(50);
  switch (dem){
  case 0:
  lcd.setCursor(0,0);       
   lcd.print("refe"); 
   break;    
  case 1:
   lcd.setCursor(0,0);       
   lcd.print("Kp");  
   break; 
  case 2:
   lcd.setCursor(0,0);       
   lcd.print("Ki");  
   break;  
  case 3:
   lcd.setCursor(0,0);       
   lcd.print("Kd");  
   break;       
  }
   if (key){
      if(key == '*')
         key = '.';
      temp[i] = key;
      if(key != '#')
      inString += (char)key;
      lcd.setCursor(i,1);
      if(dem < 4)       
      lcd.print(key);       
      i++;
      if(key == '#'){
        switch(dem){
          case 0:
          {
          if(active==0)
          Vitridat = inString.toFloat();
          else
          Tocdodat = inString.toFloat();
          }
          break;    
          case 1:
          Kp = inString.toFloat();
          break;    
          case 2:
          Ki = inString.toFloat();
          break;    
          case 3:
          Kd = inString.toFloat();
          break;    
        }
        inString = "";
         temp[i - 1] = NULL;
             for(int j = 0;j < i;j++)
             temp[j] = NULL;
             i = 0;
             lcd.clear();
             dem++;
             delay(50);
           }
   }
  }
}

// Rotary Encoder Inputs
#define Rot_1_CLK 31
#define Rot_1_SW 30
#define Rot_1_DT 29

#define Rot_2_CLK 35
#define Rot_2_SW 34
#define Rot_2_DT 33

#define Rot_3_CLK 39
#define Rot_3_SW 38
#define Rot_3_DT 37

#define Brightness 49

#define LCD_Contrast_pin 3

#include <LiquidCrystal.h>
#include "Rotary.h";

Rotary rotary1 = Rotary(Rot_1_CLK, Rot_1_DT);
Rotary rotary2 = Rotary(Rot_2_CLK, Rot_2_DT);
Rotary rotary3 = Rotary(Rot_3_CLK, Rot_3_DT);

String cmdString = "";

int secondsOfInactivityShutOff = 10;

unsigned long timeOfEvent;

unsigned long lastButtonPress = 0;
LiquidCrystal lcd(7, 8, 9, 10, 11, 12);
void setup() {
  timeOfEvent = millis();

  pinMode(Brightness, OUTPUT);
  digitalWrite(Brightness,1);
  
  // Set Contrast of LCD to white
  pinMode(LCD_Contrast_pin, OUTPUT);
  analogWrite(LCD_Contrast_pin, 70);
  
  // Set encoder pins as inputs 1
  pinMode(Rot_1_SW, INPUT);

  // Encoder 2
  pinMode(Rot_2_SW, INPUT);

  // Encoder 3
  pinMode(Rot_3_SW, INPUT);

  // Setup Serial Monitor
  Serial.begin(9600);


  lcd.begin(16, 2);
  
  // Print a message to the LCD.
  lcd.print("Starting...");
  delay(1500);
  lcd.clear();
  lcd.print("Connecting...");
  
  Serial.begin(9600);
}

void loop() {
  
  if (Serial.available())
  {
    char ch = Serial.read();
    if(ch == ';')
    { 
      processCommand(cmdString);
      cmdString = "";
    }
    else
    {
      cmdString = cmdString + ch;
    }
  }
  
  processEncoder1();
  processEncoder2();
  processEncoder3();

  if(timeOfEvent + 1000 * secondsOfInactivityShutOff < millis())
  {
    // Shut off display aftter doing nothing for 10s
    digitalWrite(Brightness,0);
  }
}

void processEncoder1()
{
  unsigned char result = rotary1.process();
  if(result == DIR_CCW)
  {
    eventWake();
    Serial.print("UP-1;");
  }
  else if(result == DIR_CW)
  {
    // Encoder is rotating CW so increment
    eventWake();
    Serial.print("DOWN-1;");
  }
  
  // Read the button state
  int btnState = digitalRead(Rot_1_SW);
  
  //If we detect LOW signal, button is pressed
  if (btnState == LOW) {
    //if 50ms have passed since last LOW pulse, it means that the
    //button has been pressed, released and pressed again
    if (millis() - lastButtonPress > 50)
    {
      eventWake();
      Serial.print("PRESS-1;");
    }
  
    // Remember last button press event
    lastButtonPress = millis();
  }
}

void processEncoder2()
{
  unsigned char result = rotary2.process();
  if(result == DIR_CCW)
  {
    eventWake();
    Serial.print("UP-2;");
  }
  else if(result == DIR_CW)
  {
    // Encoder is rotating CW so increment
    eventWake();
    Serial.print("DOWN-2;");
  }
  
  
  // Read the button state
  int btnState = digitalRead(Rot_2_SW);
  
  //If we detect LOW signal, button is pressed
  if (btnState == LOW) {
    //if 50ms have passed since last LOW pulse, it means that the
    //button has been pressed, released and pressed again
    if (millis() - lastButtonPress > 50)
    {
      eventWake();
      Serial.print("PRESS-2;");
    }
  
    // Remember last button press event
    lastButtonPress = millis();
  }
}

void processEncoder3()
{
  unsigned char result = rotary3.process();
  if(result == DIR_CCW)
  {
    eventWake();
    Serial.print("UP-3;");
  }
  else if(result == DIR_CW)
  {
    eventWake();
    // Encoder is rotating CW so increment
    Serial.print("DOWN-3;");
  }

  // Read the button state
  int btnState = digitalRead(Rot_3_SW);
  
  //If we detect LOW signal, button is pressed
  if (btnState == LOW) {
    //if 50ms have passed since last LOW pulse, it means that the
    //button has been pressed, released and pressed again
    if (millis() - lastButtonPress > 50)
    {
      eventWake();
      Serial.print("PRESS-3;");
    }
  
    // Remember last button press event
    lastButtonPress = millis();
  }
}

// Available commands : ROW1: or ROW2:
void processCommand(String command)
{
  eventWake();
  timeOfEvent = millis();
  if(command.startsWith("ROW1:"))
  {
    displayRow1(command.substring(5));
  }
  else if(command.startsWith("ROW2:"))
  {
    displayRow2(command.substring(5));
  }
  else if(command.startsWith("ECHO:"))
  {
    Serial.print(command + ';');
  }
}

void displayRow1(String text)
{
  //text = padStringTo16(text);
  lcd.setCursor(0, 0);
  lcd.print(text);
}

void displayRow2(String text)
{
  text = padStringTo16(text);
  for(int i = 0; i <= 15; i++)
  {
    lcd.setCursor(i,1);
    if(text.charAt(i) == '*')
    {
      lcd.print((char)255);
    }
    else
    {
      lcd.print(text.charAt(i));
    } 
  }
}

String padStringTo16(String strIn)
{
  for(int i = strIn.length(); i <= 16; i++)
  {
    strIn += " ";
  }
  return strIn;
}

void eventWake()
{
  timeOfEvent = millis();
  digitalWrite(Brightness,1);
}

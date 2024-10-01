// Jasmin's ~Official~ 3-DoF Hoxel and 4-DoF FingerPrint Control Code for Unity -- Version 1

#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

// Define Pressure Input
int pressure1 = A8;
int pressure2 = A9;
int pressure3 = A10;
int pressure4 = A11;
int pressure5 = A12;
int pressure6 = A13;
int pressure7 = A14;
int pressure8 = A15;

// Define Solenoid Pins
int sol1 = A0;
int sol2 = A1;
int sol3 = A2;
int sol4 = A3;
int sol5 = A4;
int sol6 = A5;
int sol7 = A6;
int sol8 = A7;

int enableLS = 40;  // Assuming digital pin 40 for enable

// Initialize Pumps (PWM)
int pwmPump1 = 9;
int pwmPump2 = 16;
int pwmPump3 = 7;
int pwmPump4 = 6;
int pwmPump5 = 5;
int pwmPump6 = 4;
int pwmPump7 = 3;
int pwmPump8 = 2;

// Define timing variables
float maxFreq = 128;
float minFreq = 0.5;
float minPeriod = 1 / maxFreq;
float maxPeriod = 1 / minFreq;
float t2 = 0;
float timeError = 0.05;

// Define range of pump speeds
float minPumpSpeed = 10; // a
float maxPumpSpeed = 100; // b

// Set initial values
float minForce = 0.1;
float maxForce = 150.0;
float X0Prev = minForce;
float Y0Prev = minForce;
float Z0Prev = minForce;
float magF0Prev = 0.0;
float X1Prev = minForce;
float Y1Prev = minForce;
float Z1Prev = minForce;
float magF1Prev = 0.0;

// Actutation Parameters
float kZ = 1.0; // z-gain
float scale = 4.0; // normal-shear roatio

// Utility functions ===========================================================
// Return the voltage on the current pin in V
float getVoltage(int pin) 
{
  float VRef = 3.3;
  int resolution = 1023;  // ADC resolution for Arduino (10-bit) | Changed from 65535 on CircuitPython
  return analogRead(pin) * VRef / resolution;
}

// Outputs pressure in PSI, input is 3.3V scale voltage measured at pressure ADC
float getPressure(float V) 
{
  float VSup = 5;
  float R1 = 19.6;
  float R2 = 10;
  float VOut = V * ((R1 + R2) / R1);

  // P in PSI
  float PMin = 0;
  float PMax = 15;
  return 6.89476 * ((VOut - (0.1 * VSup)) * (PMax - PMin) / (0.8 * VSup) + PMin);
}

int duty2bits(float duty) 
{
  return (int)(duty * 255 / 100);  // Arduino uses 8-bit PWM
  // Changed from (duty * 65535 / 100) in CircuitPython
}

void pumpsOn(int d) 
{
  analogWrite(pwmPump1, d);
  analogWrite(pwmPump2, d);
  analogWrite(pwmPump3, d);
  analogWrite(pwmPump4, d);
  analogWrite(pwmPump5, d);
  analogWrite(pwmPump6, d);
  analogWrite(pwmPump7, d);
  analogWrite(pwmPump8, d);
}

// Everything is off and air is released from the line
void exhaustHoxel0() 
{
  analogWrite(pwmPump1, 0);
  analogWrite(pwmPump2, 0);
  analogWrite(pwmPump3, 0);
  analogWrite(pwmPump4, 0);
}

void exhaustHoxel1() 
{
  analogWrite(pwmPump5, 0);
  analogWrite(pwmPump6, 0);
  analogWrite(pwmPump7, 0);
  analogWrite(pwmPump8, 0);
}

// Calculate pump speed depending on commanded force
float getPumpSpeed(float force) 
{
  force = 15 * abs(force);
  return (force >= maxPumpSpeed) ? maxPumpSpeed : force;
}

// ------ Device 0 ------
void x0Pos(int d) 
{
  analogWrite(pwmPump1, d);
  analogWrite(pwmPump2, d);
  analogWrite(pwmPump3, 0);
  analogWrite(pwmPump4, 0);
}

void x0Neg(int d) 
{
  analogWrite(pwmPump1, 0);
  analogWrite(pwmPump2, 0);
  analogWrite(pwmPump3, d);
  analogWrite(pwmPump4, d);
}

void y0Pos(int d) 
{
  analogWrite(pwmPump1, 0);
  analogWrite(pwmPump2, d);
  analogWrite(pwmPump3, d);
  analogWrite(pwmPump4, 0);
}

void y0Neg(int d) 
{
  analogWrite(pwmPump1, d);
  analogWrite(pwmPump2, 0);
  analogWrite(pwmPump3, 0);
  analogWrite(pwmPump4, d);
}

void z0(int d) 
{
  analogWrite(pwmPump1, d);
  analogWrite(pwmPump2, d);
  analogWrite(pwmPump3, d);
  analogWrite(pwmPump4, d);
}

// ------ Device 1 ------

void x1Pos(int d) 
{
  analogWrite(pwmPump5, 0);
  analogWrite(pwmPump6, 0);
  analogWrite(pwmPump7, d);
  analogWrite(pwmPump8, d);
}

void x1Neg(int d) 
{
  analogWrite(pwmPump5, d);
  analogWrite(pwmPump6, d);
  analogWrite(pwmPump7, 0);
  analogWrite(pwmPump8, 0);
}

void y1Pos(int d) 
{
  analogWrite(pwmPump5, 0);
  analogWrite(pwmPump6, d);
  analogWrite(pwmPump7, d);
  analogWrite(pwmPump8, 0);
}

void y1Neg(int d) 
{
  analogWrite(pwmPump5, d);
  analogWrite(pwmPump6, 0);
  analogWrite(pwmPump7, 0);
  analogWrite(pwmPump8, d);
}

void z1(int d) 
{
  analogWrite(pwmPump5, d);
  analogWrite(pwmPump6, d);
  analogWrite(pwmPump7, d);
  analogWrite(pwmPump8, d);
}

void setup() 
{
  pinMode(enableLS, OUTPUT);
  digitalWrite(enableLS, HIGH);

  // // Set the pump frequency
  // analogWriteFrequency(pwmPump1, 8000);
  // analogWriteFrequency(pwmPump2, 8000);
  // analogWriteFrequency(pwmPump3, 8000);
  // analogWriteFrequency(pwmPump4, 8000);
  // analogWriteFrequency(pwmPump5, 8000);
  // analogWriteFrequency(pwmPump6, 8000);
  // analogWriteFrequency(pwmPump7, 8000);
  // analogWriteFrequency(pwmPump8, 8000);

  Serial.begin(115200);

  exhaustHoxel0();
  exhaustHoxel1();
}

float X0, Y0, Z0, magF0, shear0, M0, X1, Y1, Z1, magF1, shear1, M1;

void loop() 
{
  //Serial.println(Serial.available());
  if (Serial.available() > 0) 
  {
    //Serial.println("READING");
    String data = Serial.readStringUntil('\n');
    Serial.println((String)"DATA: "+ data);
    sscanf(data.c_str(), "%f %f %f %f %f %f %f %f %f %f %f %f", &X0, &Y0, &Z0, &magF0, &shear0, &M0, &X1, &Y1, &Z1, &magF1, &shear1, &M1);

    moveHoxel0(X0, Y0, Z0, magF0, shear0);
    //moveHoxel1(X1, Y1, Z1, magF1, shear1);
    delay(5);
  } 
  else 
  {
    delay(5);
  }

    // moveHoxel0(0, 0, 10, 10, 0);
    // delay(1000);
    // moveHoxel0(0, 0, 0, 0, 0);
    // delay(1000);

}

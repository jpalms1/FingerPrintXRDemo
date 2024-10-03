// Move the Hoxels in "off-angles" -- not at the n*45 deg angles 
void moveDiagonals0(float X0, float Y0, float Z0, float magF0, float kZ) 
{
    int d1, d2, d3, d4; //pump1Speed, pump2Speed, pump3Speed, pump4Speed;

    // Check the conditions for X0 and Y0
    if (X0 >= 0.0 && Y0 >= 0.0) 
    {
        d1 = duty2bits(kZ * getPumpSpeed(Z0));
        d2 = duty2bits(getPumpSpeed(magF0)) + duty2bits(kZ * getPumpSpeed(Z0));
        d3 = duty2bits(kZ * getPumpSpeed(Z0));
        d4 = duty2bits(kZ * getPumpSpeed(Z0));
    }
    else if (X0 >= 0.0 && Y0 < 0.0) 
    {
        d1 = duty2bits(getPumpSpeed(magF0)) + duty2bits(kZ * getPumpSpeed(Z0));
        d2 = duty2bits(kZ * getPumpSpeed(Z0));
        d3 = duty2bits(kZ * getPumpSpeed(Z0));
        d4 = duty2bits(kZ * getPumpSpeed(Z0));
    }
    else if (X0 < 0.0 && Y0 >= 0.0) 
    {
        d1 = duty2bits(kZ * getPumpSpeed(Z0));
        d2 = duty2bits(kZ * getPumpSpeed(Z0));
        d3 = duty2bits(getPumpSpeed(magF0)) + duty2bits(kZ * getPumpSpeed(Z0));
        d4 = duty2bits(kZ * getPumpSpeed(Z0));
    }
    else 
    {
        d1 = duty2bits(kZ * getPumpSpeed(Z0));
        d2 = duty2bits(kZ * getPumpSpeed(Z0));
        d3 = duty2bits(kZ * getPumpSpeed(Z0));
        d4 = duty2bits(getPumpSpeed(magF0)) + duty2bits(kZ * getPumpSpeed(Z0));
    }

    // Set the PWM duty cycles
    analogWrite(pwmPump1, d1);
    analogWrite(pwmPump2, d2);
    analogWrite(pwmPump3, d3);
    analogWrite(pwmPump4, d4);
}

void moveHoxel0(float X0, float Y0, float Z0, float magF0, float shear0) 
{
  if (magF0 <= minForce) 
  {
    //Serial.println((String)"too low " + magF0);
    exhaustHoxel0();
  } 
  else 
  {
    //Serial.println((String)"tryiong " + magF0);
    z0(duty2bits(kZ * getPumpSpeed(Z0)));
    // if (abs(Z0) >= scale * shear0)  // Normal force only
    // {
    //   z0(duty2bits(kZ * getPumpSpeed(Z0)));
    // } 
    // else 
    // {
    //   if (abs(X0) >= scale * abs(Y0)) // X is dominant -> Move X
    //   {
    //     (X0 >= 0.0) ? x0Pos(duty2bits(getPumpSpeed(X0))) : x0Neg(duty2bits(getPumpSpeed(X0)));
    //   } 
    //   else if (abs(Y0) >= scale * abs(X0)) // Y is dominant -> Move Y
    //   {
    //     (Y0 >= 0.0) ? y0Pos(duty2bits(getPumpSpeed(Y0))) : y0Neg(duty2bits(getPumpSpeed(Y0)));
    //   } 
    //   else 
    //   {
    //     moveDiagonals0(X0, Y0, Z0, magF0, kZ);
    //   }
    // }
  }
}

// Move the Hoxels in "off-angles" -- not at the n*45 deg angles 
void moveDiagonals1(float X1, float Y1, float Z1, float magF1, float kZ) 
{
    int d5, d6, d7, d8; //pump5Speed, pump6Speed, pump7Speed, pump8Speed;

    // Check the conditions for X1 and Y1
    if (X1 >= 0.0 && Y1 >= 0.0) 
    {
        d5 = duty2bits(kZ * getPumpSpeed(Z1));
        d6 = duty2bits(getPumpSpeed(magF1)) + duty2bits(kZ * getPumpSpeed(Z1));
        d7 = duty2bits(kZ * getPumpSpeed(Z1));
        d8 = duty2bits(kZ * getPumpSpeed(Z1));
    }
    else if (X1 >= 0.0 && Y1 < 0.0) 
    {
        d5 = duty2bits(getPumpSpeed(magF1)) + duty2bits(kZ * getPumpSpeed(Z1));
        d6 = duty2bits(kZ * getPumpSpeed(Z1));
        d7 = duty2bits(kZ * getPumpSpeed(Z1));
        d8 = duty2bits(kZ * getPumpSpeed(Z1));
    }
    else if (X1 < 0.0 && Y1 >= 0.0) 
    {
        d5 = duty2bits(kZ * getPumpSpeed(Z1));
        d6 = duty2bits(kZ * getPumpSpeed(Z1));
        d7 = duty2bits(getPumpSpeed(magF1)) + duty2bits(kZ * getPumpSpeed(Z1));
        d8 = duty2bits(kZ * getPumpSpeed(Z1));
    }
    else 
    {
        d5 = duty2bits(kZ * getPumpSpeed(Z1));
        d6 = duty2bits(kZ * getPumpSpeed(Z1));
        d7 = duty2bits(kZ * getPumpSpeed(Z1));
        d8 = duty2bits(getPumpSpeed(magF1)) + duty2bits(kZ * getPumpSpeed(Z1));
    }

    // Set the PWM duty cycles
    analogWrite(pwmPump5, d5);
    analogWrite(pwmPump6, d6);
    analogWrite(pwmPump7, d7);
    analogWrite(pwmPump8, d8);
}


void moveHoxel1(float X1, float Y1, float Z1, float magF1, float shear1) 
{
  if (magF1 <= minForce) 
  {
    exhaustHoxel1();
  } 
  else 
  {
    if (abs(Z1) >= scale * shear1) 
    {
      z1(duty2bits(kZ * getPumpSpeed(Z1)));
    } 
    else 
    {
      if (abs(X1) >= scale * abs(Y1)) 
      {
        (X1 >= 0) ? x1Pos(duty2bits(getPumpSpeed(X1))) : x1Neg(duty2bits(getPumpSpeed(X1)));
      } 
      else if (abs(Y1) >= scale * abs(X1)) 
      {
        (Y1 >= 0) ? y1Pos(duty2bits(getPumpSpeed(Y1))) : y1Neg(duty2bits(getPumpSpeed(Y1)));
      } 
      else 
      {
        moveDiagonals1(X1, Y1, Z1, magF1, kZ);
      }
    }
  }
}

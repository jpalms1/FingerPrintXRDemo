String inputString = "";   // String to hold incoming data
float values[12];          // Array to store the 12 float values
bool stringComplete = false;

//  The values being stored for the devices
float X0, Y0, Z0, magF0, shear0, M0; // Device 0
float X1, Y1, Z1, magF1, shear1, M1; // Device 1


void setup() {
  Serial.begin(115200);
  inputString.reserve(200);  // Reserve some memory for the input string
}

void loop() {
  // Check if a complete string has been received
  if (stringComplete) {
    // Split and convert the string to float values
    int startIndex = 0;
    int spaceIndex = inputString.indexOf(' ');
    int count = 0;

    while (spaceIndex != -1 && count < 12) {
      String subString = inputString.substring(startIndex, spaceIndex);
      values[count] = subString.toFloat();  // Convert substring to float
      count++;
      startIndex = spaceIndex + 1;
      spaceIndex = inputString.indexOf(' ', startIndex);
    }

    // Handle the last value after the last space
    if (count < 12) {
      String subString = inputString.substring(startIndex);
      values[count] = subString.toFloat();
    }
    
    // Store the values to their appropriate variables
    X0 = values[0];
    Y0 = values[1];
    Z0 = values[2];
    magF0 = values[3];
    shear0 = values[4];
    M0 = values[5];

    X1 = values[6];
    Y1 = values[7];
    Z1 = values[8];
    magF1 = values[9];
    shear1 = values[10];
    M1 = values[11];

    // // Print the values to verify
    // String readout = "Return: ";
    // for (int i = 0; i < 12; i++) {
    //   readout = readout + " " = String(values[i]);
    //   // Serial.print(values[i]);
    //   // Serial.print(" ");
    // }
    // Serial.print(readout);

    // Clear the input string and reset the flag
    inputString = "";
    stringComplete = false;
  }

  // Other code can run here
}

// Interrupt-based handler for receiving serial input
void serialEvent() {
  while (Serial.available()) {
    char inChar = (char)Serial.read();
    inputString += inChar;

    // Check if the input string is complete (terminated by newline)
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}

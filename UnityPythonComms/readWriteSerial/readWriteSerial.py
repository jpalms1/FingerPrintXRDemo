# Written by: Jasmin Elena Palmer (jasminp@stanford.edu)

import serial
import time

# Current Serial port
mcu_port = "COM3"  # change this to Arduino/teensy's port
baud_rate = 115200

mcu = serial.Serial(port=mcu_port, baudrate=baud_rate, timeout=0.1)
# Wait for the board to initialize
time.sleep(1)
print("BOARD INITIALIZED")

while 1:

    value = mcu.readline()
    value = str(value, "utf-8").split()
    print(value)


    # mcu.write(value.encode())


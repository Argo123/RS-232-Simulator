To communicate with slaves, you must create an ICommStream (currently, this library supports for SerialStream via serial port). After that, call the request function corresponds witch each function code. The request function will send a request to slave and waiting for the response, at that time, other requests will be pending until the current one is completed.

More details: https://bitbucket.org/sontx/dotnet-utils/src/master/Code4Bugs.Utils/IO/Modbus/

Example 1: create ICommStream and communicate with slaves via COM port.

var serialPort = new SerialPort("COM1");
var stream = new SerialStream(serialPort);  
var responseBytes = stream.RequestFunc3(slaveId, dataAddress, registerCount);

Example 2: request data from slave by func 3

// send request and waiting for response
// the request needs: slaveId, dataAddress, registerCount
var responseBytes = stream.RequestFunc3(0x11, 0x006B, 0x0003);
// extract the content part (the most important in the response)
var data = responseBytes.ToResponseFunc3().Data;
var heartRate = data.ToInt16(0);// 1 register contains heart rate value, bytes from 0 to 1
var stepCount = data.ToInt32(2);// 2 register contain step count value, bytes from 2 to 5

Example 3: write data to slave by func 16

var unixTime = DateTime.Now.ToUnixEpoch();// convert current time to unix epoch
// the request needs: slaveId, dataAddress, writeValue
var responseBytes = stream.RequestFunc16(0x11, 0x0001, unixTime.ToBytes());
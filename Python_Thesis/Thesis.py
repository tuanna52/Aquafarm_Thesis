from digi.xbee.devices import XBeeDevice
from time import strftime, sleep
from requests import post
from json import dumps
from ast import literal_eval

#current location name
location = 'Hanoi';

###Xbee Port
##Port = 'COM5'
#Setting the baud rate
Baud_Rate = 9600

#Firebase Url
firebase_url = 'https://aquafarm-c9b41.firebaseio.com/'


def main():
    Number = 0
    while 1:
        try:
            Port = "COM" + str(Number)
            device = XBeeDevice(Port, Baud_Rate)
            device.open()
            break
        except:
            print(f"Cannot open {Port}")
            Number = Number + 1
            sleep(0.5)

    def my_data_received_callback(xbee_message):
        try:
            #Read received data
            predata_utf = xbee_message.data.decode("utf8")
            data0 = literal_eval(predata_utf)

            #Extract data
            EC = float(data0['EC'][0:data0['EC'].find(',')])
            #EC = data0['EC']
            pH = data0['pH'] + 3.5
            DO = data0['DO']
            RTD = data0['RTD']
            

            #current time and date
            time_hhmmss = strftime('%H:%M:%S')
            date_mmddyyyy = strftime('%d/%m/%Y')
            
            
	    #insert record
            data = {'Conductivity': EC, 'pH': pH, 'DO': DO, 'Temperature': RTD, 'date': date_mmddyyyy, 'time': time_hhmmss}
            result = post(firebase_url + '/' + location + '/AquaFarm.json', data=dumps(data))
            

        except:
            print('Something has been wrong!')

    # Add the callback.
    device.add_data_received_callback(my_data_received_callback)

if __name__ == '__main__':
    main()

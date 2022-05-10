#define SENSOR1 7
#define SENSOR2 A3
#define SENSOR3 A1
#define SENSOR4 A2
#define SENSOR5 A5
#define SENSOR6 A0
#define SENSOR7 11
#define SENSOR8 A4

#define RELAY 6
#define RED_LED 4
#define GREEN_LED 5
#define SLOT_SENSOR 3

enum stages
{
	WAIT_FOR_SLOT_SENSOR = 0,
	CHECK_ALL_SENSORS = 1,
	WAIT_FOR_END = 2
};

int stage;

void setup()
{ 
	Serial.begin(9600);
	pinMode(SENSOR1, INPUT);
	pinMode(SENSOR2, INPUT);
	pinMode(SENSOR3, INPUT);
	pinMode(SENSOR4, INPUT);
	pinMode(SENSOR5, INPUT);
	pinMode(SENSOR6, INPUT);
	pinMode(SENSOR7, INPUT);
	pinMode(SENSOR8, INPUT);
  
	pinMode(RELAY,OUTPUT);
	pinMode(RED_LED, OUTPUT);
	pinMode(GREEN_LED,OUTPUT);
	pinMode(SLOT_SENSOR, INPUT);

	digitalWrite(RED_LED, HIGH);
	stage = WAIT_FOR_SLOT_SENSOR;
}

void loop()
{
	switch(stage)
	{
		case WAIT_FOR_SLOT_SENSOR:
/* debug port 
      Serial.print("SLOT_SENSOR = ");
      Serial.println(digitalRead(SLOT_SENSOR));
*/
			if(!digitalRead(SLOT_SENSOR)) stage = CHECK_ALL_SENSORS;
		//delay(2000); //delay for debug
			break;
		case CHECK_ALL_SENSORS:
// Debug ports
/*      delay(2000);   
		Serial.print("INSIDE PERF = ");
		Serial.println(digitalRead(SLOT_SENSOR)); 
		Serial.print("SENSOR1 = ");
		Serial.println(digitalRead(SENSOR1));
		Serial.print("SENSOR2 = ");
		Serial.println(analogRead(SENSOR2));
		Serial.print("SENSOR3 = ");
		Serial.println(analogRead(SENSOR3));
		Serial.print("SENSOR4 = ");
		Serial.println(analogRead(SENSOR4));
		Serial.print("SENSOR5 = ");
		Serial.println(analogRead(SENSOR5));
		Serial.print("SENSOR6 = ");
		Serial.println(analogRead(SENSOR6));
		Serial.print("SENSOR7 = ");
		Serial.println(digitalRead(SENSOR7));
		Serial.print("SENSOR8 = ");
		Serial.println(analogRead(SENSOR8));	
		delay(2000);
*/
			if(digitalRead(SENSOR1)
				&& analogRead(SENSOR2)>400 
				&& analogRead(SENSOR3)>400 
				&& analogRead(SENSOR4)>600 
				&& analogRead(SENSOR5)>100 
				&& analogRead(SENSOR6)>300 
				&& digitalRead(SENSOR7) 
				&& analogRead(SENSOR8)>500)		
			{
				digitalWrite(RELAY, HIGH);
				digitalWrite(RED_LED,LOW);
				digitalWrite(GREEN_LED,HIGH);
				stage = WAIT_FOR_END;
				delay(60000);
			}
			else
			{
				stage = WAIT_FOR_SLOT_SENSOR;
			}
			break;
		case WAIT_FOR_END:
			if(digitalRead(SLOT_SENSOR))  
			{
				digitalWrite(RELAY, LOW);
				digitalWrite(RED_LED,HIGH);
				digitalWrite(GREEN_LED,LOW);
				stage = WAIT_FOR_SLOT_SENSOR;
			}
			break;
	}
}

// Define Trig and Echo pin:
#define trigPin 2
#define echoPin 4
#define rLED 10
#define bLED 11
#define gLED 12
#define yLED 13

void setup() {
  // put your setup code here, to run once:
  pinMode(3,INPUT_PULLUP);
  pinMode(5,INPUT_PULLUP);
  pinMode(6,INPUT_PULLUP);
  pinMode(9,INPUT_PULLUP);
  pinMode(rLED,OUTPUT);
  pinMode(bLED,OUTPUT);
  pinMode(gLED,OUTPUT);
  pinMode(yLED,OUTPUT);
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  Serial.begin(115200);
}

void loop() {
  // put your main code here, to run repeatedly:
  
  delay(50);
  String message = Serial.readStringUntil('\n');
  readButtons();
  message.trim();
  if(message.length()>0){
    setLED(message);
  }
  readSensor();
}

void readButtons(){
  int b0,b2,b1,b3;
  b0 = digitalRead(3);
  b1 = digitalRead(5);
  b2 = digitalRead(6);
  b3 = digitalRead(9);
  if(b0==LOW){
    Serial.println("b0");
  }
  if(b1==LOW){
    Serial.println("b1");
  }
  if(b2==LOW){
    Serial.println("b2");
  }
  if(b3==LOW){
    Serial.println("b3");
  }

}

void setLED(String led){
  if(led == "3"){
    digitalWrite(yLED,HIGH);
    digitalWrite(gLED,LOW);
    digitalWrite(bLED,LOW);
    digitalWrite(rLED,LOW);
  }
  else if(led == "2"){
    digitalWrite(yLED,LOW);
    digitalWrite(gLED,HIGH);
    digitalWrite(bLED,LOW);
    digitalWrite(rLED,LOW);
  }
  else if(led == "1"){
    digitalWrite(yLED,LOW);
    digitalWrite(gLED,LOW);
    digitalWrite(bLED,HIGH);
    digitalWrite(rLED,LOW);
  }
  else if(led == "0"){
    digitalWrite(yLED,LOW);
    digitalWrite(gLED,LOW);
    digitalWrite(bLED,LOW);
    digitalWrite(rLED,HIGH);
  }
}

void readSensor(){
  // Define variables:
  long duration;
  int distance;

    // Clear the trigPin by setting it LOW:
    digitalWrite(trigPin, LOW);
    delayMicroseconds(5);

    // Trigger the sensor by setting the trigPin high for 10 microseconds:
    digitalWrite(trigPin, HIGH);
    delayMicroseconds(10);
    digitalWrite(trigPin, LOW);

    // Read the echoPin, pulseIn() returns the duration (length of the pulse) in microseconds:
    duration = pulseIn(echoPin, HIGH);
    // Calculate the distance:
    distance = duration * 0.034 / 2;

    // Print the distance on the Serial Monitor (Ctrl+Shift+M):
    Serial.println(distance);
}

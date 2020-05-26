//include <Arduino.h>

//==================================
//=VARIABLES========================
//==================================
int echo_front = 4;
int echo_right = 10;
int echo_left = 2;

int trig_front = 3;
int trig_right = 11;
int trig_left = 5;

int MOVE_FORWARDS = 1;
int MOVE_BACKWARDS = 2;
int TURN_LEFT = 3;
int TURN_RIGHT = 4;
int WAIT = 5;
int STOP = 6;
int PANIC = 7;
int BACK = 8;
int State;

int CriticalDistance = 15;
int CriticalDistanceSide = 20;        // This var is experimental. Might not be needed in the future.
int CriticalDistanceWhenTurning = 10; // This should be about 20-30% larger than CriticalDistance in order to eliminate laggy rotation.
int SecondsToPanic = 5;
bool IsApproachingLeft;
bool IsApproachingRight;
int LastForwardCommand;

int RightDistanceHistory[5];
int LeftDistanceHistory[5];
int hisLen = 5;

//==================================
//==================================
//==================================

class InputManager
{
private:
public:
  int upDistance;
  int leftDistance;
  int rightDistance;
  int HistoryCounter = 0;

  InputManager()
  {
    for (int i = 0; i < hisLen; i++)
    {
      RightDistanceHistory[i] = 100;
      LeftDistanceHistory[i] = 100;
    }
  }

  void FixedUpdate()
  {
    RaycastCheckUpdate();
  }

  int dist(int echoPin, int trigPin)
  {
    digitalWrite(trigPin, LOW);
    delayMicroseconds(2);

    digitalWrite(trigPin, HIGH);
    delayMicroseconds(10);
    digitalWrite(trigPin, LOW);

    double duration = pulseIn(echoPin, HIGH);

    double distance = duration * 0.034 / 2;

    return distance;
  }

  void RaycastCheckUpdate()
  {
    upDistance = dist(echo_front, trig_front);
    leftDistance = dist(echo_left, trig_left);
    rightDistance = dist(echo_right, trig_right);

    // Keep track of previous distances.
    RightDistanceHistory[HistoryCounter] = rightDistance;
    LeftDistanceHistory[HistoryCounter] = leftDistance;
    HistoryCounter++;

    if (HistoryCounter >= 5)
    {
      HistoryCounter = 0;
    }
  }
};

class AFDCP
{
private:
  int a;

public:
  AFDCP()
  {

    pinMode(7, OUTPUT);
    pinMode(8, OUTPUT);
    pinMode(9, OUTPUT);
  }

  void STOP()
  {
    digitalWrite(7, LOW);
    digitalWrite(8, LOW);
    digitalWrite(9, LOW);
  }

  void GO()
  {
    digitalWrite(7, LOW);
    digitalWrite(8, LOW);
    digitalWrite(9, HIGH);
  }

  void BACK()
  {
    digitalWrite(7, LOW);
    digitalWrite(8, HIGH);
    digitalWrite(9, LOW);
  }

  void LEFT()
  {
    digitalWrite(7, LOW);
    digitalWrite(8, HIGH);
    digitalWrite(9, HIGH);
  }

  void RIGHT()
  {
    digitalWrite(7, HIGH);
    digitalWrite(8, LOW);
    digitalWrite(9, LOW);
  }
};

//CLASSES
AFDCP car;
InputManager im;
// AFDCP - Arduino & FEZ Digital Communication Protocol
// ----------------------------------------

void setup()
{
  car.STOP();

  //-----------------------
  pinMode(trig_front, OUTPUT);
  pinMode(echo_front, INPUT);
  //-----------------------
  pinMode(trig_right, OUTPUT);
  pinMode(echo_right, INPUT);
  //-----------------------
  pinMode(trig_left, OUTPUT);
  pinMode(echo_left, INPUT);
  //-----------------------

  State = MOVE_FORWARDS;
  LastForwardCommand = 0;

  Serial.begin(9600);
}

void loop()
{
  im.FixedUpdate();
  HandleStates();
  IsApproachingLeft = IsApproaching(LeftDistanceHistory);
  IsApproachingRight = IsApproaching(RightDistanceHistory);

  delay(1);
}

void Waiting()
{
  if (im.upDistance >= CriticalDistance && !IsApproachingRight && !IsApproachingLeft)
  {
    State = MOVE_FORWARDS;
  }
  else if (IsApproachingRight)
  {
    State = TURN_LEFT;
  }
  else if (IsApproachingLeft)
  {
    State = TURN_RIGHT;
  }
  else if (im.upDistance <= CriticalDistance)
  {
    
    State = BACK;
  }
  else
  {
    State = PANIC;
  }
}

void MoveForwards()
{
  if (im.upDistance <= CriticalDistance || IsApproachingRight || IsApproachingLeft)
  {
    State = WAIT;
  }

  car.GO(); // Move vehicle forwards
}

void MoveBackwards()
{
  car.BACK(); // Move vehicle backwards
  delay(1000);
  im.FixedUpdate();
  if (im.leftDistance >= im.rightDistance)
  {
      car.LEFT();
      delay(1000);
  }
  else
  {
      car.RIGHT();
      delay(1000);
  }
  State = MOVE_FORWARDS;
}

void TurnLeft()
{
  if (im.rightDistance >= CriticalDistanceSide && im.upDistance >= CriticalDistanceWhenTurning)
  {
    State = WAIT;
  }
  car.LEFT(); // Turn vehicle left
}

void TurnRight()
{
  if (im.leftDistance >= CriticalDistanceSide && im.upDistance >= CriticalDistanceWhenTurning)
  {
    State = WAIT;
  }
  car.RIGHT(); // Turn vehicle right
}

void Stop()
{
  car.STOP();
}

void Panic()
{
  car.STOP();
}

void HandleStates()
{
  switch (State)
  {
  case 1:
    MoveForwards();
    break;
  case 2:
    MoveBackwards();
    break;
  case 3:
    TurnLeft();
    break;
  case 4:
    TurnRight();
    break;
  case 5:
    Waiting();
    break;
  case 7:
    Panic();
    break;
  case 8:
    MoveBackwards();
    break;
  default:
    Stop();
    break;
  }
}

bool IsApproaching(int *DistanceHistory)
{
  bool IsApproaching = false;
  int SmallerDistanceCounter = 0;
  for (int i = 1; i < hisLen; i++)
  {
    if (DistanceHistory[i] <= CriticalDistanceSide && DistanceHistory[i] < DistanceHistory[i - 1])
    {
      SmallerDistanceCounter++;
    }
  }
  if (SmallerDistanceCounter >= hisLen / 2)
  {
    IsApproaching = true;
  }

  return IsApproaching;
}

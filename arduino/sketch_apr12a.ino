#include <Arduino.h>

class AFDCP {
  private:
      int a;
  public:
    AFDCP() {
      
      pinMode(7, OUTPUT);
      pinMode(8, OUTPUT);
      pinMode(9, OUTPUT);
    }
  
    void STOP() {
       digitalWrite(7, LOW);
       digitalWrite(8, LOW);
       digitalWrite(9, LOW);
    }
    
    void GO() {
       digitalWrite(7, LOW);
       digitalWrite(8, LOW);
       digitalWrite(9, HIGH);
    }
    
    void BACK() {
       digitalWrite(7, LOW);
       digitalWrite(8, HIGH);
       digitalWrite(9, LOW);
    }
    
    void LEFT() {
       digitalWrite(7, LOW);
       digitalWrite(8, HIGH);
       digitalWrite(9, HIGH);
    }
    
    void RIGHT() {
       digitalWrite(7, HIGH);
       digitalWrite(8, LOW);
       digitalWrite(9, LOW);
    }
    
};

//CLASSES
AFDCP car;
// AFDCP - Arduino & FEZ Digital Communication Protocol
// ----------------------------------------

void setup() {
  car.STOP();
}

void loop() {
  delay(3000);
  
  car.GO();
  delay(3000);

  car.STOP();
  delay(3000);
  
  car.RIGHT();
  delay(2000);

    car.STOP();

}

using UnityEngine;
using System.Collections;

public class countDown : MonoBehaviour {
//countdownTimer: methods to handle a countdown timer                 phuzzy


//it is always assumed that there is a guiText item available for the display output

//PRIVATE MEMBERS
private bool  b_timer_active; //switch to start/stop timer
private FIXME_VAR_TYPE f_timer_done; //method to be called when timer runs down
private float fl_start_time; //start time (in seconds)
private float fl_time_left; //time left (in seconds)

//PUBLIC METHODS
void  getFlRemainingTime (){ //get the time remaining on the clock
   return fl_time_left;
}

void  setTimerDoneAction (f_method_fp){ //set the method to be called when the timer is done
   f_timer_done = f_method_fp;
}

void  setTimerState ( bool b_active_fp  ){  //set the active state of the timer
   b_timer_active = b_active_fp;
}

void  setStartTime ( float fl_time_fp  ){ //set the starting value for the countdown
   fl_start_time = fl_time_fp;
}

void  Update (){
   if (b_timer_active) { //check to see if the timer is "on"
      
         doCountdown(); //decrement the time 
     
   }
}

//PRIVATE METHODS
private void  doCountdown (){ //
   if (fl_start_time) { //make sure that we had a starting time value before conting down
      fl_time_left = fl_start_time - Time.time;
      fl_time_left = Mathf.Max(0, fl_time_left); //don't let the time fall below 0.0f
      GetComponent.<GUIText>().text = outReadableTime(fl_time_left); //display the time to the GUI
      if (fl_time_left == 0.0f) { //if time has run out, deactivate the timer and call the followup method
         b_timer_active = false;
         if (f_timer_done) { //only call the followup method if we had one
            f_timer_done();
         }
      }
   } else {
      Debug.Log("countdownTimer needs a value set for fl_time_left");
   }
}

private void  outReadableTime ( float fl_time_fp  ){ //format the floating point seconds to M:S
   int i_minutes;
   int i_seconds;
   int i_time;
   string s_timetext;
   i_time = fl_time_fp;
   i_minutes = i_time / 60;
   i_seconds = i_time % 60;
   s_timetext = i_minutes.ToString() + ":";
   s_timetext = s_timetext + i_seconds.ToString();
   return s_timetext;
}





}
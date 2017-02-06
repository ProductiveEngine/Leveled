//countdownTimer: methods to handle a countdown timer                 phuzzy


//it is always assumed that there is a guiText item available for the display output

//PRIVATE MEMBERS
private var b_timer_active : boolean; //switch to start/stop timer
private var f_timer_done; //method to be called when timer runs down
private var fl_start_time : float; //start time (in seconds)
private var fl_time_left : float; //time left (in seconds)

//PUBLIC METHODS
function getFlRemainingTime() { //get the time remaining on the clock
   return fl_time_left;
}

function setTimerDoneAction(f_method_fp) { //set the method to be called when the timer is done
   f_timer_done = f_method_fp;
}

function setTimerState(b_active_fp : boolean) { //set the active state of the timer
   b_timer_active = b_active_fp;
}

function setStartTime(fl_time_fp : float) { //set the starting value for the countdown
   fl_start_time = fl_time_fp;
}

function Update() {
   if (b_timer_active) { //check to see if the timer is "on"
      
         doCountdown(); //decrement the time 
     
   }
}

//PRIVATE METHODS
private function doCountdown() { //
   if (fl_start_time) { //make sure that we had a starting time value before conting down
      fl_time_left = fl_start_time - Time.time;
      fl_time_left = Mathf.Max(0, fl_time_left); //don't let the time fall below 0.0
      GetComponent.<GUIText>().text = outReadableTime(fl_time_left); //display the time to the GUI
      if (fl_time_left == 0.0) { //if time has run out, deactivate the timer and call the followup method
         b_timer_active = false;
         if (f_timer_done) { //only call the followup method if we had one
            f_timer_done();
         }
      }
   } else {
      Debug.Log("countdownTimer needs a value set for fl_time_left");
   }
}

private function outReadableTime(fl_time_fp : float) { //format the floating point seconds to M:S
   var i_minutes : int;
   var i_seconds : int;
   var i_time : int;
   var s_timetext : String;
   i_time = fl_time_fp;
   i_minutes = i_time / 60;
   i_seconds = i_time % 60;
   s_timetext = i_minutes.ToString() + ":";
   s_timetext = s_timetext + i_seconds.ToString();
   return s_timetext;
}





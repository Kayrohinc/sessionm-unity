package com.sessionm.unity;

import android.util.Log;
import android.view.Menu;

import com.sessionm.api.AchievementActivity;
import com.sessionm.api.AchievementActivity.AchievementDismissType;
import com.sessionm.api.AchievementActivityIllegalStateException;
import com.sessionm.api.AchievementData;
import com.sessionm.api.Activity;
import com.sessionm.api.SessionM.ActivityType;
import com.sessionm.api.SessionM;
import com.unity3d.player.UnityPlayerActivity;

public class BaseActivity extends UnityPlayerActivity {

    private final static String TAG = "SessionM.Unity";
    
    private final SessionM sessionM = SessionM.getInstance();
    
    public static final void setCallbackGameObjectName(String name) {
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, "Callback game object name: " + name);
        }
        SessionMListener.getInstance().setCallbackGameObjectName(name);
    }
   
    // Convenience Unity/Android bridge methods
    
    public boolean presentActivity(ActivityType type) {
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, "Present activity: " + type);
        }

        return sessionM.presentActivity(type);
    }
    
    public boolean isActivityAvailable(ActivityType type) {
        boolean available = false;
        if(sessionM.getSessionState() == SessionM.State.STARTED_ONLINE) {
            if(type == ActivityType.ACHIEVEMENT) {
                available = sessionM.getUnclaimedAchievement() != null;
            } else {
                available = true;
            }
        }
        return available;
    }
    
    public int getUnclaimedAchievementCount() {
        return sessionM.getUser().getUnclaimedAchievementCount();
    }

    public String getUnclaimedAchievementJSON() {
        String json = null;
        AchievementData achievement = sessionM.getUnclaimedAchievement();
        if(achievement != null) {
            json = SessionMListener.getAchievementJSON(achievement);
        }
        return json;
    }

    public void notifyCustomAchievementPresented() {
        AchievementData achievement = sessionM.getUnclaimedAchievement();
        if(achievement == null) {
            // this cannot happen 
            Log.e(TAG, this + ".notifyCustomAchievementPresented(): Null achievement");
            return;
        }
        
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, this + ".notifyCustomAchievementPresented(), achievement: " + achievement);
        }
        
        AchievementActivity activity = new AchievementActivity(achievement);
        try {
            activity.notifyPresented();
        } catch (AchievementActivityIllegalStateException e) {
            Log.e(TAG, this + ".notifyCustomAchievementPresented()", e);
        }
    }
    
    public void notifyCustomAchievementCancelled() {
        Activity activity = sessionM.getCurrentActivity();
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, this + ".notifyCustomAchievementCancelled(), activity: " + activity);
        }

        if(activity instanceof AchievementActivity) {
            AchievementActivity a = (AchievementActivity)activity;
            try {
                a.notifyDismissed(AchievementDismissType.CANCELLED);
            } catch (AchievementActivityIllegalStateException e) {
                Log.e(TAG, this + ".notifyCustomAchievementCancelled()", e);
            }
        }
    }

    public void notifyCustomAchievementClaimed() {
        Activity activity = sessionM.getCurrentActivity();
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, this + ".notifyCustomAchievementClaimed(), activity: " + activity);
        }

        if(activity instanceof AchievementActivity) {
            AchievementActivity a = (AchievementActivity)activity;
            try {
                a.notifyDismissed(AchievementDismissType.CLAIMED);
            } catch (AchievementActivityIllegalStateException e) {
                Log.e(TAG, this + ".notifyCustomAchievementClaimed()", e);
            }
        }
    }
    
    // Activity 
    
    @Override
    protected void onStart() {
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, this + ".onStart()");
        }

        super.onStart();
        sessionM.onActivityStart(this);
    }

    @Override
    protected void onRestart() {
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, this + ".onRestart()");
        }       
        super.onRestart();
    }

    @Override
    protected void onResume() {
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, this + ".onResume()");
        }

        super.onResume();
        sessionM.onActivityResume(this);
    }

    @Override
    protected void onPause() {
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, this + ".onPause()");
        }

        sessionM.onActivityPause(this);
        super.onPause();
    }

    @Override
    protected void onStop() {
        if(Log.isLoggable(TAG, Log.DEBUG)) {
            Log.d(TAG, this + ".onStop()");
        }

        super.onStop();
        sessionM.onActivityStop(this);
    }

    @Override
    public boolean onPrepareOptionsMenu(Menu menu) {
        sessionM.dismissActivity();
        return super.onPrepareOptionsMenu(menu);
    }
}
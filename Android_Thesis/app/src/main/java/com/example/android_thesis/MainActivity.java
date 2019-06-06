package com.example.android_thesis;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Intent;
import android.os.Build;
import android.support.v4.app.NotificationCompat;
import android.support.v4.app.NotificationManagerCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.animation.Animation;
import android.view.animation.LinearInterpolator;
import android.widget.TextView;

import com.google.firebase.FirebaseApp;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.text.DecimalFormat;

public class MainActivity extends AppCompatActivity {

    private DatabaseReference mReference;

    private TextView mPH;
    private TextView mDO;
    private TextView mEC;
    private TextView mRTD;

    String CHANNEL_ID = "my_channel_01";// The id of the channel.
    int notificationId = 0;             // The notification ID

    // Variables to control how long is the next warning
    private int pH_Next_Warning = 30;       // ~300s for the next warning
    private int DO_Next_Warning = 30;       // ~300s for the next warning
    private int RTD_Next_Warning = 30;     // ~300s for the next warning

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Initialize Database
        mReference = FirebaseDatabase.getInstance().getReference("Hanoi").child("AquaFarm");

        // Initialize View
        mPH = findViewById(R.id.pH_value);
        mDO = findViewById(R.id.DO_value);
        mEC = findViewById(R.id.EC_value);
        mRTD =  findViewById(R.id.temp_value);
    }

    @Override
    public void onStart() {
        super.onStart();

        // Create a notification channel
        createNotificationChannel();

        // Add value event listener
        ValueEventListener Listener = new ValueEventListener() {

            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                for(DataSnapshot ds : dataSnapshot.getChildren()) {
                    Double PH_Double = ds.child("pH").getValue(Double.class);
                    Double DO_Double = ds.child("DO").getValue(Double.class);
                    Integer EC_Int = ds.child("Conductivity").getValue(Integer.class);
                    Double RTD_Double = ds.child("Temperature").getValue(Double.class);

                    mPH.setText(new DecimalFormat("##.##").format(PH_Double));
                    mDO.setText(new DecimalFormat("##.##").format(DO_Double));
                    mEC.setText(Integer.toString(EC_Int));
                    mRTD.setText(new DecimalFormat("##.##").format(RTD_Double));

                    pH_Next_Warning++;
                    DO_Next_Warning++;
                    RTD_Next_Warning++;

                    if ((PH_Double > 9 || PH_Double < 7.5) && pH_Next_Warning >= 30) {
                        addNotification_pH();
                        pH_Next_Warning = 0;
                    }

                    if ((DO_Double < 5) && DO_Next_Warning >= 30) {
                        addNotification_DO();
                        DO_Next_Warning = 0;
                    }

                    if ((RTD_Double > 30 || RTD_Double < 22) && RTD_Next_Warning >= 30) {
                        addNotification_RTD();
                        RTD_Next_Warning = 0;
                    }
                }
            }

            @Override
            public void onCancelled(DatabaseError databaseError) {
                System.out.println("The read failed: " + databaseError.getCode());
            }
        };

        mReference.addValueEventListener(Listener);
    }

    // Create the channel for notification
    private void createNotificationChannel() {
        // Create the NotificationChannel, but only on API 26+ because
        // the NotificationChannel class is new and not in the support library
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            CharSequence name = getString(R.string.channel_name);
            String description = getString(R.string.channel_description);
            int importance = NotificationManager.IMPORTANCE_DEFAULT;
            NotificationChannel channel = new NotificationChannel(CHANNEL_ID, name, importance);
            channel.setDescription(description);
            // Register the channel with the system; you can't change the importance
            // or other notification behaviors after this
            NotificationManager notificationManager = getSystemService(NotificationManager.class);
            notificationManager.createNotificationChannel(channel);
        }
    }

    // Create a notification when the pH indicator exceed the ideal zone
    private void addNotification_pH()
    {
        // Create an explicit intent for an Activity in your app
        Intent intent = new Intent(this, MainActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, intent, 0);

        NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(this, CHANNEL_ID)
                .setSmallIcon(R.drawable.warning)
                .setContentTitle("Warning")
                .setContentText("The pH indicator is in dangerous zone!")
                .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                // Set the intent that will fire when the user taps the notification
                .setContentIntent(pendingIntent)
                .setAutoCancel(true);

        NotificationManagerCompat notificationManager = NotificationManagerCompat.from(this);

        // notificationId is a unique int for each notification that you must define
        notificationManager.notify(notificationId, mBuilder.build());
    }

    // Create a notification when the DO indicator exceed the ideal zone
    private void addNotification_DO()
    {
        // Create an explicit intent for an Activity in your app
        Intent intent = new Intent(this, MainActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, intent, 0);

        NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(this, CHANNEL_ID)
                .setSmallIcon(R.drawable.warning)
                .setContentTitle("Warning")
                .setContentText("The DO indicator is in dangerous zone!")
                .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                // Set the intent that will fire when the user taps the notification
                .setContentIntent(pendingIntent)
                .setAutoCancel(true);

        NotificationManagerCompat notificationManager = NotificationManagerCompat.from(this);

        // notificationId is a unique int for each notification that you must define
        notificationManager.notify(notificationId, mBuilder.build());
    }

    // Create a notification when the temperature indicator exceed the ideal zone
    private void addNotification_RTD()
    {
        // Create an explicit intent for an Activity in your app
        Intent intent = new Intent(this, MainActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, intent, 0);

        NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(this, CHANNEL_ID)
                .setSmallIcon(R.drawable.warning)
                .setContentTitle("Warning")
                .setContentText("The temperature indicator is in dangerous zone!")
                .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                // Set the intent that will fire when the user taps the notification
                .setContentIntent(pendingIntent)
                .setAutoCancel(true);

        NotificationManagerCompat notificationManager = NotificationManagerCompat.from(this);

        // notificationId is a unique int for each notification that you must define
        notificationManager.notify(notificationId, mBuilder.build());
    }
}

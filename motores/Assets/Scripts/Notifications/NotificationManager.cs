using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Notifications.Android;
public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance 
    {
        get; private set;
    }

    AndroidNotificationChannel notifChannel;
    private void Awake()
    {
        if(Instance != this && Instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        notifChannel = new AndroidNotificationChannel()
        {
            Id = "reminder_notif_ch",
            Name = "Reminder Notification",
            Description = "Reminder to login",
            Importance = Importance.High
        };
        AndroidNotificationCenter.RegisterNotificationChannel(notifChannel);
        DisplayNotification("Vuelve", "Juega otra vez", 
            IconSelecter.icon_reminder, IconSelecter.icon_reminderbig, DateTime.Now.AddHours(24));
    }
    public int DisplayNotification(string title, string text, IconSelecter iconSmall, IconSelecter iconLarge, DateTime fireTime)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.SmallIcon = iconSmall.ToString();
        notification.LargeIcon = iconLarge.ToString();
        notification.FireTime = fireTime;

        return AndroidNotificationCenter.SendNotification(notification, notifChannel.Id);


    }

    public void CancelNotification(int id)
    {
        AndroidNotificationCenter.CancelScheduledNotification(id);
    }

    public enum IconSelecter
    {
        icon_reminder,
        icon_reminderbig

    }
}

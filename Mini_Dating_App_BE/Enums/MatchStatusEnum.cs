namespace Mini_Dating_App_BE.Enums
{
    public enum MatchStatusEnum
    {
        //Have a match but not accepted to date yet => Scheduling (User accepted match and setting Availability if not setting)
        Pending,
        //Accepted match but waiting for free slot to schedule or waiting for other user to accepted => Scheduled
        Scheduling,
        //When both Users are accepted and have free slot
        Scheduled,
        //Accepted but reject free slot wait for another free slot
        Rescheduled
    }
}

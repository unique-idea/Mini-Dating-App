namespace Mini_Dating_App_BE.Enums
{
    public enum MatchStatusEnum
    {
        //Have a match but both users not accepted to date yet 
        Matched,
        //One user accepted match but waiting for other user to accepted 
        Pending,
        //Both user accepted but no have free slot wait for one of user reset avaibalitity and rechedule again
        NoSlotFound,
        //When both Users are accepted and have free slot
        Scheduled,
  
    }
}

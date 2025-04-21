namespace SportHive.Services.Interfaces
{
 public interface ISaveDataDb
 { 
    Task SaveDataToDb(string jsonObj,string topic);
 }
}
using DB.SportHive.Domain;

namespace SportHive.SaveServices.Interfaces
{
 public interface ISaveDataDb
 { 
     Task SaveDataUser(string jsonObj, string topic);
 }
}
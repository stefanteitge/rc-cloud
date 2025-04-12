namespace RcCloud.DateScraper.Domain.Clubs;

public interface IClubRepository
{
    Club? FindClub(string clubName);

    void Load(string path);

    void Store(string path);
    
    void Update(Club update);
    
    IEnumerable<Club> GetAll();
    
    void Load(List<Club> clubs);
}

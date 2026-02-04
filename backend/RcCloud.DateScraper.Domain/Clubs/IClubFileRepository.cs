namespace RcCloud.DateScraper.Domain.Clubs;

public interface IClubFileRepository
{
    Club? FindClub(string clubName);

    void Load(string path);
    
    Task LoadFromGithub();

    void Store(string path);
    
    void Update(Club update);
    
    IEnumerable<Club> GetAll();
    
    void Load(List<Club> clubs);
}

namespace RcCloud.DateScraper.Domain.Clubs;

public interface IClubCopyRepository
{
    Task<List<Club>> GetAll(string compilation);

    Task<bool> Store(List<Club> clubs);
}

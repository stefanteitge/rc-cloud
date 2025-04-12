using System.Text.Json;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Regions;
using RcCloud.DateScraper.Infrastructure.Clubs.Entities;

namespace RcCloud.DateScraper.Infrastructure.Clubs;

public class JsonClubRepository : IClubRepository
{
    private static readonly ClubEntity[] _seed = [
        Make("AMC Magdeburg e.V.", [], RegionReference.East),
        Make("MC Ettlingen e.V.", [], RegionReference.Central),
        Make("MC Munster e.V.", [], RegionReference.North),
        Make("MCC Rhein-Ahr e.V.", ["MCC Rhein-Ahr e.V"], RegionReference.West),
        Make("MRC-Leipzig e.V.", [], RegionReference.East),
        Make("Modellclub Flensburg e.V.", ["Modellclub Flensburg e. V."], RegionReference.North),
        Make("ORC-B Göttingen e.V.", [], RegionReference.North),
        Make("RC Speedracer e.V.", ["RC Speedracer e.V. OV-055"], RegionReference.East),
    ];

    private ClubDbEntity _clubDb = new ClubDbEntity(_seed.ToList(), DateTimeOffset.Now, "germany");

    public Club? FindClub(string clubName)
    {
        var entity = FindClubEntity(clubName);

        return entity?.ToDomain();
    }

    public void Load(string path)
    {
        _clubDb.LastUpdated = DateTimeOffset.Now;
        var options = new JsonSerializerOptions(JsonSerializerOptions.Web)
        {
            WriteIndented = true
        };
        var loaded = JsonSerializer.Deserialize<ClubDbEntity>(new FileStream(path, FileMode.Open), options);

        if (loaded is null)
        {
            // seed was used
            return;
        }

        _clubDb = loaded;
    }
    
    public void Store(string path)
    {
        var options = new JsonSerializerOptions(JsonSerializerOptions.Web)
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        
        var db = new ClubDbEntity(_clubDb.Clubs.OrderBy(c => c.Name).ToList(), DateTimeOffset.Now, "germany");
        
        var bytes = JsonSerializer.SerializeToUtf8Bytes(db, options);
        File.WriteAllBytes(path, bytes);
    }

    public void Update(Club update)
    {
        var entity = FindClubEntity(update.Name);

        if (entity is null)
        {
            var newClub = new ClubEntity(update.Name, update.Aliases, "de", update.Region?.ToString(), update.DmcClubNumber,
                update.MyrcmClubNumbers.ToList());
            _clubDb.Clubs.Add(newClub);
            return;
        }
        
        // updates
        foreach (var alias in update.Aliases)
        {
            if (!entity.Aliases.Contains(alias))
            {
                entity.Aliases.Add(alias);
            }
        }

        // XXX we ingore conflicting updates here       
        if (update.DmcClubNumber is not null)
        {
            entity.DmcClubNumber = update.DmcClubNumber;
        }

        if (update.Region is not null)
        {
            entity.Region = update.Region.ToString();
        }
        
        // TODO: myrcm
    }

    public IEnumerable<Club> GetAll()
        => _clubDb.Clubs.Select(c => c.ToDomain()).OrderBy(c => c.Name);

    private ClubEntity? FindClubEntity(string clubName)
    {
        var entity = _clubDb.Clubs
            .FirstOrDefault(c => c.Name == clubName || c.Aliases.Contains(clubName));

        return entity;
    }

    private static ClubEntity Make(string name, string[] aliases, RegionReference regionReference)
        => new(name, aliases.ToList(), "de", regionReference.ToString(), null, []);
}

using Backend_Development_LeMarconnes_Reserveringssysteem.Repositories;

public class DAL
    {
        private readonly string _connectionString;

        public AccommodatieRepository Accommodaties { get; }
        public CampingRepository Campings { get; }
        public BoekingRepository Boekingen { get; }
        public BetalingRepository Betalingen { get; }
        public FeedbackRepository Feedbacks { get; }
        public FaciliteitRepository Faciliteiten { get; }
        public FaciliteitBlokkadeRepository FaciliteitBlokkades { get; }
        public GebruikerRepository Gebruikers { get; }
        public MedewerkerRepository Medewerkers { get; }

        public DAL(string connectionString)
        {
            _connectionString = connectionString;

            Accommodaties = new AccommodatieRepository(_connectionString);
            Campings = new CampingRepository(_connectionString);
            Boekingen = new BoekingRepository(_connectionString);
            Betalingen = new BetalingRepository(_connectionString);
            Feedbacks = new FeedbackRepository(_connectionString);
            Faciliteiten = new FaciliteitRepository(_connectionString);
            FaciliteitBlokkades = new FaciliteitBlokkadeRepository(_connectionString);
            Gebruikers = new GebruikerRepository(_connectionString);
            Medewerkers = new MedewerkerRepository(_connectionString);
        }
    }

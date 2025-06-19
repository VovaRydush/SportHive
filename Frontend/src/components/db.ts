export interface AthconsteProf {
    name: string;
    sport: string;
    photo: string;
    stats: string;
    Team: string;
    DataBirth: string;
    login: string;
    Position: string;
    Matches: string[];
    dataMathes: string; //потім вибрати один чи другий обєкт і витянути з нього дані
}
export interface Organization {
    login: string;
    photo:string;
    NameOrganization: string;
    TypeOrganozation: string;
    Description: string;
    Country: string;
    Teams: TeamIndivid[];
    OrganizationJudge: string[];
    OrganizationTrainer: string[];
    Events: string[];
}
export interface Trainer {
    FirsName: string;
    LastName: string;
    SportType: string;
    Photo: string;
    Teams: string[];
    Organizations: string[];
    DataBirth: string;
    login: string;
    Position: string;
    Matches: string[];
    stats?: Stats;
}
export interface Judge {
    login: string;
    Photo: string;
    FirsName: string;
    LastName: string;
    Category: string;
}
export interface Match {
    idMatch: string;
    NameEvent: string;
    systems: string;
    DataStart: string;
    DataEnd: string;
    sport: string;
    Time: string;
    description: string;
    status: string;
    team1: string;
    team2: string;
    score: string;
    NameWinner: string;
    loginJudge: string;
    date: string;
    LocationName: string;
    Tour: string;
    Group: string;
    dataFotball: dataMatchFotball[];
    AddInformation: string;
}
export interface Event {
    NameEvent: string;
    TypeMatch:string;
    description: string;
    date: string;
    endDate: string;
    location: string;
    sport: string;
    athletes: string[];
    teams: string[];
    matches: string[];
    standings: Standing[];

    
}
export interface TeamIndivid {
    name: string;
    logo: string;
    LoginTrainer: string;
    AthleteLogins: string[];
    sport: string;
    wins: string;
    stats: Stats;
    OrganizationTeam?: Organization[];
    Matches: Match[];
}
export interface Stats {
    TotalMatches: string;
    Wins: string;
    Draws: string;
    Losses: string;
    Trophies: string;
}
export interface dataMatchFotball {
    id: number,
    time: string,
    type: string,
    player: string,
    team: string,
    description: string
}
export interface fottballProfile {
    name: string;
    Matches: number;
    Wins: number;
    Draws: number;
    Losses: number;
    Goals: number;
    Assists: number;
    YellowCards: number;
    RedCards: number;
}
export interface Standing {
    participant: string;
    points: number;
    wins: number;
    draws: number;
    losses: number;
}
export interface dataMatchBox {
    namePlayer: string;
    id: string;
    time: string;
    round: number;
    type: string;
}
export interface boxlProfile {
    Matches: number;
    Wins: number;
    Knockouts: number;
    Losses: number;
    RoundsFought: number;
    AverageScorePerRound: number;
    WeightCategory: number;
}
export const users: AthconsteProf[] = [];
export const trainers: Trainer[] = [];
export const organizations: Organization[] = [];
export const judges: Judge[] = [];
export const Teams: TeamIndivid[] = [];
export const event: Event[] = [];
export const match: Match[] = [];
export const allEvents: Event[] = [];
export const allRecentResults: Match[] = [];
export const allLiveMatches: Match[] = [];
export const allUpcomingMatches: Match[] = [];
export const allTopTeams: TeamIndivid[] = [];
export const allTopAthletes: AthconsteProf[] = [];



users.push(
    // Football players (16)
    {
        name: "Олександр Іваненко",
        sport: "Футбол",
        photo: "https://randomuser.me/api/portraits/men/1.jpg",
        stats: "24 голи, 8 асистів",
        Team: "Динамо",
        DataBirth: "1995-03-15",
        login: "ivanenko_foot",
        Position: "Нападник",
        Matches: ["match1", "match2", "match3"],
        dataMathes: ""
    },
    {
        name: "Михайло Петренко",
        sport: "Футбол",
        photo: "https://randomuser.me/api/portraits/men/2.jpg",
        stats: "12 голів, 15 асистів",
        Team: "Динамо",
        DataBirth: "1993-07-22",
        login: "petrenko_foot",
        Position: "Півзахисник",
        Matches: ["match1", "match2", "match3"],
        dataMathes: ""
    },
    // ... 14 more football players (8 for Dynamo, 8 for Skify)
    
    // Basketball players (5)
    {
        name: "Марія Сидоренко",
        sport: "Баскетбол",
        photo: "https://randomuser.me/api/portraits/women/1.jpg",
        stats: "18.5 очків/гра, 5.2 підбирання",
        Team: "Олімпійці",
        DataBirth: "1997-05-10",
        login: "sidorenko_basket",
        Position: "Форвард",
        Matches: ["match4", "match5"],
        dataMathes: ""
    },
    // ... 4 more basketball players
    
    // Tennis players (4)
    {
        name: "Ігор Семенов",
        sport: "Теніс",
        photo: "https://randomuser.me/api/portraits/men/10.jpg",
        stats: "85% виграних подач",
        Team: "Стрибуни",
        DataBirth: "1990-11-30",
        login: "semenov_tennis",
        Position: "Одиночний розряд",
        Matches: ["match6", "match7"],
        dataMathes: ""
    });

trainers.push(
    {
        FirsName: "Василь",
        LastName: "Довбуш",
        SportType: "Футбол",
        Photo: "https://randomuser.me/api/portraits/men/20.jpg",
        Teams: [Teams[0], Teams[1]],
        Organizations: ["org1"],
        DataBirth: "1975-08-12",
        login: "dovbush_trainer",
        Position: "Головний тренер",
        Matches: ["match1", "match2", "match3"],
        stats: {
            TotalMatches: "156",
            Wins: "98",
            Draws: "32",
            Losses: "26",
            Trophies: "7"
        }
    },
    {
        FirsName: "Ольга",
        LastName: "Мельник",
        SportType: "Баскетбол",
        Photo: "https://randomuser.me/api/portraits/women/20.jpg",
        Teams: ["Олімпійці"],
        Organizations: ["org2"],
        DataBirth: "1982-04-05",
        login: "melnyk_trainer",
        Position: "Головний тренер",
        Matches: ["match4", "match5"],
        stats: {
            TotalMatches: "85",
            Wins: "52",
            Draws: "0",
            Losses: "33",
            Trophies: "3"
        }
    });

organizations.push(
    {
        login: "org1",
        photo: "https://logo.clearbit.com/dynamo.kiev.ua",
        NameOrganization: "ФК Динамо",
        TypeOrganozation: "Футбольний клуб",
        Description: "Професійний футбольний клуб з Києва",
        Country: "Україна",
        Teams: ["Динамо", "Скіфи"],
        OrganizationJudge: ["judge1", "judge2"],
        OrganizationTrainer: ["dovbush_trainer"],
        Events: ["event1"]
    },
    {
        login: "org2",
        photo: "https://logo.clearbit.com/nba.com",
        NameOrganization: "Баскетбольна Асоціація",
        TypeOrganozation: "Спортивна асоціація",
        Description: "Організація, що проводить баскетбольні турніри",
        Country: "Україна",
        Teams: ["Олімпійці"],
        OrganizationJudge: ["judge3"],
        OrganizationTrainer: ["melnyk_trainer"],
        Events: []
    });

    judges.push(
    {
        login: "judge1",
        Photo: "https://randomuser.me/api/portraits/men/30.jpg",
        FirsName: "Андрій",
        LastName: "Шевченко",
        Category: "ФІФА"
    },
    {
        login: "judge2",
        Photo: "https://randomuser.me/api/portraits/men/31.jpg",
        FirsName: "Олексій",
        LastName: "Коваленко",
        Category: "Національна"
    },
    {
        login: "judge3",
        Photo: "https://randomuser.me/api/portraits/women/30.jpg",
        FirsName: "Наталія",
        LastName: "Бойко",
        Category: "ФІБА"
    });

    Teams.push(
    {
        name: "Динамо",
        logo: "https://template.canva.com/EAGRnsId_m8/1/0/1600w-0cb_X-21UpE.jpg",
        LoginTrainer: "dovbush_trainer",
        AthleteLogins: ["ivanenko_foot", "petrenko_foot", /* 6 more */],
        sport: "Футбол",
        wins: "12",
        stats: {
            TotalMatches: "25",
            Wins: "18",
            Draws: "4",
            Losses: "3",
            Trophies: "2"
        },
        OrganizationTeam: [organizations[0]],
        Matches: ["match1", "match2"]
    },
    {
        name: "Скіфи",
        logo: "https://example.com/skify_logo.jpg",
        LoginTrainer: "dovbush_trainer",
        AthleteLogins: [/* 8 football players */],
        sport: "Футбол",
        wins: "8",
        stats: {
            TotalMatches: "25",
            Wins: "8",
            Draws: "10",
            Losses: "7",
            Trophies: "0"
        },
        OrganizationTeam: [organizations[0]],
        Matches: ["match1", "match3"]
    });
    // ... 3 more teams (Олімпійці, Стрибуни, Вікторія)
    event.push({
    NameEvent: "Чемпіонат міста з футболу",
    TypeMatch: "Турнір",
    description: "Щорічний чемпіонат міста серед аматорських команд",
    date: "2023-11-20",
    endDate: "2023-12-15",
    location: "Стадіон 'Юність'",
    sport: "Футбол",
    athletes: [/* all football players */],
    teams: ["Динамо", "Скіфи", "Вікторія", "Юність"],
    matches: ["match1", "match2", "match3", "match4"],
    standings: [
        { participant: "Динамо", points: 9, wins: 3, draws: 0, losses: 0 },
        { participant: "Скіфи", points: 4, wins: 1, draws: 1, losses: 1 },
        // ... other teams
    ]});

 match.push(
    // Event matches
    {
        idMatch: "match1",
        NameEvent: "Чемпіонат міста з футболу",
        systems: "Груповий етап",
        DataStart: "2023-11-20",
        DataEnd: "2023-11-20",
        sport: "Футбол",
        Time: "15:00",
        description: "Груповий етап, тур 1",
        status: "Завершено",
        team1: "Динамо",
        team2: "Скіфи",
        score: "2:1",
        NameWinner: "Динамо",
        loginJudge: "judge1",
        date: "2023-11-20",
        LocationName: "Стадіон 'Юність'",
        Tour: "Груповий етап",
        Group: "Група A",
        AddInformation: "",
        dataFotball: [
            { id: 1, time: "23", type: "Гол", player: "Олександр Іваненко", team: "Динамо", description: "Удар з лівої ноги" },
            // ... more match events
        ]
    },
    // ... match2, match3, match4 (other event matches)
    
    // Other matches
    {
        idMatch: "match5",
        NameEvent: "Товариський матч",
        systems: "Одиночна гра",
        DataStart: "2023-11-25",
        DataEnd: "2023-11-25",
        sport: "Теніс",
        Time: "12:00",
        description: "Товариська зустріч",
        status: "Завершено",
        team1: "Стрибуни",
        team2: "Форхенди",
        score: "6:4, 6:3",
        NameWinner: "Стрибуни",
        loginJudge: "judge3",
        date: "2023-11-25",
        LocationName: "Тенісний клуб 'Чемпіон'",
        Tour: "",
        Group: "",
        AddInformation: "",
        dataFotball: []
    });
    
    allLiveMatches.push(
    // Currently ongoing matches
    {
        idMatch: "match8",
        NameEvent: "Кубок ліги",
        systems: "Плей-оф",
        DataStart: "2023-11-28",
        DataEnd: "2023-11-28",
        sport: "Футбол",
        Time: "17:30",
        description: "1/4 фіналу",
        status: "Live",
        team1: "Динамо",
        team2: "Вікторія",
        score: "1:1",
        NameWinner: "",
        loginJudge: "judge2",
        date: "2023-11-28",
        LocationName: "Стадіон 'Центральний'",
        Tour: "1/4 фіналу",
        Group: "",
        AddInformation: "",
        dataFotball: [
            { id: 1, time: "15", type: "Гол", player: "Михайло Петренко", team: "Динамо", description: "Пенальті" },
            { id: 2, time: "42", type: "Гол", player: "Іван Сидоренко", team: "Вікторія", description: "Удар з гри" }
        ]
    });


 allUpcomingMatches.push(
    // Future matches
    {
        idMatch: "match9",
        NameEvent: "Чемпіонат міста з футболу",
        systems: "Півфінал",
        DataStart: "2023-12-05",
        DataEnd: "2023-12-05",
        sport: "Футбол",
        Time: "16:00",
        description: "Півфінальний матч",
        status: "Заплановано",
        team1: "Динамо",
        team2: "Юність",
        score: "",
        NameWinner: "",
        loginJudge: "judge1",
        date: "2023-12-05",
        LocationName: "Стадіон 'Юність'",
        Tour: "Півфінал",
        Group: "",
        AddInformation: "",
        dataFotball: []
    });
    // ... more upcoming match

allRecentResults.push(
    // Past matches
    {
        idMatch: "match1",
        NameEvent: "Чемпіонат міста з футболу",
        systems: "Груповий етап",
        DataStart: "2023-11-20",
        DataEnd: "2023-11-20",
        sport: "Футбол",
        Time: "15:00",
        description: "Груповий етап, тур 1",
        status: "Завершено",
        team1: "Динамо",
        team2: "Скіфи",
        score: "2:1",
        NameWinner: "Динамо",
        loginJudge: "judge1",
        date: "2023-11-20",
        LocationName: "Стадіон 'Юність'",
        Tour: "Груповий етап",
        Group: "Група A",
        AddInformation: "",
        dataFotball: []
    });

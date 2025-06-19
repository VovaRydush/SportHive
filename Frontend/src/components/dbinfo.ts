import { allEvents, allLiveMatches, allRecentResults, allTopAthletes, allTopTeams, allUpcomingMatches, match } from './db';
allEvents.push({
    NameEvent: "Чемпіонат міста з футболу", date: "2023-11-20", sport: "Футбол", teams: ["Динамо", "Скіфи"],
    description: "",
    endDate: "",
    location: "",
    matches: [],
    standings: [],
    TypeMatch: '',
    athletes: []
});
    
    allRecentResults.push(
      {
          team1: "Динамо", team2: "Вікторія", score: "2:1", date: "2023-11-15", sport: "Футбол",
          idMatch: "0",
          NameEvent: "",
          systems: "",
          DataStart: "",
          DataEnd: "",
          Time: "",
          description: "",
          status: "",
          NameWinner: "",
          loginJudge: "",
          LocationName: "",
          Tour: "1",
          Group: "1",
          AddInformation: "",
          dataFotball: []
      },
      { team1: "Олімпійці", team2: "Стрімкі", score: "89:76", date: "2023-11-14", sport: "Баскетбол",
         idMatch: "1",
          NameEvent: "",
          systems: "",
          DataStart: "",
          DataEnd: "",
          Time: "",
          description: "",
          status: "",
          NameWinner: "",
          loginJudge: "",
          LocationName: "",
          Tour: "1",
          Group: "1",
          AddInformation: "",
          dataFotball: []
        },
      { team1: "Стрибуни", team2: "Форхенди", score: "6:4, 6:3", date: "2023-11-13", sport: "Теніс",
         idMatch: "2",
          NameEvent: "",
          systems: "",
          DataStart: "",
          DataEnd: "",
          Time: "",
          description: "",
          status: "",
          NameWinner: "",
          loginJudge: "",
          LocationName: "",
          Tour: "4",
          Group: "5",
          AddInformation: "",
          dataFotball: []
    },
      { team1: "Вікторія", team2: "Скіфи", score: "1:1", date: "2023-11-12", sport: "Футбол", idMatch: "3",
        NameEvent: "",
          systems: "",
          DataStart: "",
          DataEnd: "",
          Time: "",
          description: "",
          status: "",
          NameWinner: "",
          loginJudge: "",
          LocationName: "",
          Tour: "6",
          Group: "1",
          AddInformation: "",
          dataFotball: []
         }
    );
allTopTeams.push(
      {
          name: "Динамо", sport: "Футбол", wins: "12", logo: "https://static-cse.canva.com/blob/847064/29.jpg",
          LoginTrainer: "",
          AthleteLogins: [],
          Matches: [],
          stats: {
            TotalMatches: "0",
            Wins: "0",
            Draws: "0",
            Losses: "0",
            Trophies: "0"
        }
      },
      { name: "Олімпійці", sport: "Баскетбол", wins: "8", logo: "https://static-cse.canva.com/blob/847064/29.jpg",
        LoginTrainer: "",
          AthleteLogins: [],
          Matches: [],
          stats: {
            TotalMatches: "0",
            Wins: "0",
            Draws: "0",
            Losses: "0",
            Trophies: "0"
        }
       },
      { name: "Стрибуни", sport: "Теніс", wins: "5", logo: "https://static-cse.canva.com/blob/847064/29.jpg",
        LoginTrainer: "",
          AthleteLogins: [],
          Matches: [],stats: {
            TotalMatches: "0",
            Wins: "0",
            Draws: "0",
            Losses: "0",
            Trophies: "0"
        }
         },
      { name: "Вікторія", sport: "Футбол", wins: "7", logo: "https://static-cse.canva.com/blob/847064/29.jpg",
        LoginTrainer: "",
          AthleteLogins: [],
          Matches: [],
          stats: {
            TotalMatches: "0",
            Wins: "0",
            Draws: "0",
            Losses: "0",
            Trophies: "0"
        }
      });

    allTopAthletes.push(
      {
          name: "Олександр Іваненко", sport: "Футбол", stats: "24 голи", photo: "https://template.canva.com/EAGZeVbaBh4/1/0/1600w-FQWnYg_IWXU.jpg",
          Team: "",
          DataBirth: "",
          login: "",
          Position: "",
          Matches: [],
          dataMathes: ""
      },
      { name: "Марія Петренко", sport: "Баскетбол", stats: "18.5 очків/гра", photo: "https://static-cse.canva.com/blob/847064/29.jpg",
        Team: "",
          DataBirth: "",
          login: "",
          Position: "",
          Matches: [],
          dataMathes: ""
      },
      { name: "Ігор Семенов", sport: "Теніс", stats: "85% виграних подач", photo: "https://static-cse.canva.com/blob/847064/29.jpg",
        Team: "",
          DataBirth: "",
          login: "",
          Position: "",
          Matches: [],
          dataMathes: ""
      });
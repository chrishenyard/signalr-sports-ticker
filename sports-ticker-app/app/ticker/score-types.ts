export interface Game {
  id: string;
  opponents: Opponent[];
}

export interface Opponent {
  id: string;
  team: Team;
  pointsPerQuarter: number[];
  home: boolean;
  totalPoints: number;
}

export interface Player {
  id: string;
  firstName: string;
  lastName: string;
  stats: Stats;
  headshot: string;
}

export interface Scores {
  games: Game[];
  gameClock: GameClock;
}

export interface Stats {
  points: number;
  rebounds: number;
  assists: number;
}

export interface Team {
  id: string;
  name: string;
  players: Player[];
  highPoints: Player;
  highRebounds: Player;
  highAssists: Player;
  home: Standings;
  away: Standings;
  combined: Standings;
  logo: string;
}

export interface Standings {
  wins: number;
  losses: number;
}

export interface GameClock {
  tickerInterval: number;
  quarter: number;
  quarterTime: number;
  quarterBreakTime: number;
  halfBreakTime: number;
  timeout: boolean;
  finalBuzzer: boolean;
  inProgress: boolean;
  timeRemaining: number;
}

export interface HubReconnectingProps {
  retryAttempt: number;
  retryMessage: string;
  maxRetries: number;
}

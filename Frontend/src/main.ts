import { NavigationManager } from './components/NavigationManager';

export function bootstrapSportHive() {
  const nav = new NavigationManager();
  nav.init();
}

import './styles/main.css';
import { NavigationManager } from './components/NavigationManager';

document.addEventListener('DOMContentLoaded', () => {
  const nav = new NavigationManager();
  nav.init();
});

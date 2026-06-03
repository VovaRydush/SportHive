import { TournamentsPage } from './TournamentsPage';

export class HomePage {
  private container: HTMLElement;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  async render() {
    this.container.innerHTML = `<div id="home-tournaments-root"></div>`;
    await new TournamentsPage('home-tournaments-root').render();
  }
}

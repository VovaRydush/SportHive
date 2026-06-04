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
    await new TournamentsPage(this.container.id).render();
  }
}

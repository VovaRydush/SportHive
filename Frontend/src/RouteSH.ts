type Route = {
  path: RegExp;
  view: any;
  roles: string[];
};

export class Router {
  private routes: Route[];
  private rootElement: HTMLElement;
  private userRole: string;

  constructor(routes: Route[], rootSelector: string) {
    const el = document.querySelector<HTMLElement>(rootSelector);
    if (!el) throw new Error(`Element "${rootSelector}" not found`);
    this.rootElement = el;
    this.routes = routes;
    this.userRole = localStorage.getItem('userRole') || 'guest';

    this.init();
  }

  private init() {
    window.addEventListener('popstate', () => this.handleRoute());
    document.body.addEventListener('click', e => {
      const target = e.target as HTMLElement;
      if (target.matches('[data-link]')) {
        e.preventDefault();
        const href = target.getAttribute('href');
        if (href) {
          history.pushState(null, '', href);
          this.handleRoute();
        }
      }
    });

    this.handleRoute();
  }

  public async handleRoute() {
    const path = window.location.pathname;
    for (const route of this.routes) {
      const match = path.match(route.path);
      if (match) {
        if (!route.roles.includes(this.userRole)) {
          this.rootElement.innerHTML = `<h1>403 - Доступ заборонено</h1>`;
          return;
        }
        const view = new route.view(match.slice(1));
        this.rootElement.innerHTML = await view.getHtml();
        return;
      }
    }
    this.rootElement.innerHTML = `<h1>404 - Не знайдено</h1>`;
  }
}

import { HomePage } from "./components/HomePage";

document.getElementById('homePagest')?.addEventListener('click', () => {
  const homePage = new HomePage('app');
  homePage.render();
});
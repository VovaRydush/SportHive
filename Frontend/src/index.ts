// src/index.ts
import UserPage from "./core/userPage";

document.addEventListener("DOMContentLoaded", async () => {
    const userPage = new UserPage();
    const app = document.getElementById("app");

    if (app) {
        app.appendChild(await userPage.render());
    }
});

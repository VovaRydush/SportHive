// src/core/userPage.ts
import UserService from "./userService";
import User from "./user";

export default class UserPage {
    private userService: UserService;
    private container: HTMLElement;

    constructor() {
        this.userService = new UserService();
        this.container = document.createElement("div");
    }

    async render(): Promise<HTMLElement> {
        const users: User[] = await this.userService.getUsers();
        
        this.container.innerHTML = "<h2>Список користувачів</h2>";

        users.forEach(user => {
            const userElement = document.createElement("p");
            userElement.textContent = `ID: ${user.id}, Email: ${user.email}, Підтверджений: ${user.isEmailConfirmed}`;
            this.container.appendChild(userElement);
        });

        return this.container;
    }
}

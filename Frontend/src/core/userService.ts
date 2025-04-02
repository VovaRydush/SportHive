import User from "./user";

export default class UserService {
    private apiUrl: string = "http://localhost:/users";

    async getUsers(): Promise<User[]> {
        const response = await fetch(this.apiUrl);
        const data = await response.json();

        return data.map((user: any) => new User(user.id, user.email, user.isEmailConfirmed));
    }
}

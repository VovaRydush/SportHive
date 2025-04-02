// src/core/user.ts
export default class User {
    id: number;
    email: string;
    isEmailConfirmed: boolean;
    
    constructor(id: number, email: string, isEmailConfirmed: boolean) {
        this.id = id;
        this.email = email;
        this.isEmailConfirmed = isEmailConfirmed;
    }
}

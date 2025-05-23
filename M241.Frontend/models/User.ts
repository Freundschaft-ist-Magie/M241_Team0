class User {
  id?: number;
  userName: string;
  email: string;
  password: string;

  constructor(id: number, userName: string, email: string, password: string) {
    this.id = id;
    this.userName = userName;
    this.email = email;
    this.password = password;
  }
}

export default User;

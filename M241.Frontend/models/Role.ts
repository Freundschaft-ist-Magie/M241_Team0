class Role {
  id: number;
  name: string;
  read: boolean;
  write: boolean;
  manage: boolean;

  constructor(
    id: number,
    name: string,
    read: boolean,
    write: boolean,
    manage: boolean
  ) {
    this.id = id;
    this.name = name;
    this.read = read;
    this.write = write;
    this.manage = manage;
  }
}

export default Role;

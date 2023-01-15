CREATE TABLE cards(
    id VARCHAR(36) PRIMARY KEY,
    name VARCHAR(30) NOT NULL,
    type VARCHAR(10) NOT NULL,
    element VARCHAR(10) NOT NULL,
    damage DOUBLE PRECISION NOT NULL,
    owner_name VARCHAR(30) NOT NULL,
    chosen BOOLEAN NOT NULL
);

CREATE TABLE packages(
    id SERIAL PRIMARY KEY,
    card1id VARCHAR(36) NOT NULL,
    card2id VARCHAR(36) NOT NULL,
    card3id VARCHAR(36) NOT NULL,
    card4id VARCHAR(36) NOT NULL,
    card5id VARCHAR(36) NOT NULL,
);

ALTER TABLE packages
ADD CONSTRAINT card1_fk
FOREIGN KEY (card1id) 
REFERENCES cards(id);

ALTER TABLE packages
ADD CONSTRAINT card2_fk
FOREIGN KEY (card2id)
REFERENCES cards(id);

ALTER TABLE packages
ADD CONSTRAINT card3_fk
FOREIGN KEY (card3id)
REFERENCES cards(id);

ALTER TABLE packages
ADD CONSTRAINT card4_fk
FOREIGN KEY (card4id)
REFERENCES cards(id);

ALTER TABLE packages
ADD CONSTRAINT card5_fk
FOREIGN KEY (card5id)
REFERENCES cards(id);

CREATE TABLE users(
    id SERIAL PRIMARY KEY,
    username VARCHAR(20) NOT NULL,
    password VARCHAR(70) NOT NULL,
    coins INT,
    token VARCHAR(100),
    elo INT NOT NULL,
    games INT NOT NULL,
    wins INT NOT NULL,
    losses INT NOT NULL
    UNIQUE(username)
);

CREATE TABLE profiles(
    id SERIAL PRIMARY KEY,
    user_fun VARCHAR(40) NOT NULL,
    name VARCHAR(30),
    bio VARCHAR(200),
    image VARCHAR(10),
    UNIQUE (user_fun)
);

ALTER TABLE profiles
ADD CONSTRAINT user_fun_fk
FOREIGN KEY (user_fun)
REFERENCES users(username);
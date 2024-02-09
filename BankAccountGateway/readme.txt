
Заполнение бд юзерами

INSERT INTO "Clients"  ("FullName", "PhoneNumber" , "PasswordHash" , "PasswordSalt")
VALUES 
    ('User1', '111-111-1111', 'passwordhash1', 'salt1'),
    ('User2', '222-222-2222', 'passwordhash2', 'salt2'),
    ('User3', '333-333-3333', 'passwordhash3', 'salt3');
    
    
Добавление аккаунтов
INSERT INTO "Accounts" ("AccountNumber", "AccountType", "ClientId")
VALUES
    (12345678901, 0, 1),
    (23456789012, 1, 1),
    (34567890123, 2, 1),
    (45678901234, 0, 1),
    (56789012345, 1, 1),
    (67890123456, 0, 2),
    (78901234567, 1, 2),
    (89012345678, 2, 2),
    (90123456789, 0, 2),
    (12345678901, 1, 2),
    (23456789012, 0, 3),
    (34567890123, 1, 3),
    (45678901234, 2, 3),
    (56789012345, 0, 3),
    (67890123456, 1, 3);

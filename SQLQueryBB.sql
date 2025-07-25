IF OBJECT_ID('ReviewTable', 'U') IS NOT NULL
DROP TABLE ReviewTable;

IF OBJECT_ID('ProductOrderTable', 'U') IS NOT NULL
DROP TABLE ProductOrderTable;

IF OBJECT_ID('OrderTable', 'U') IS NOT NULL
DROP TABLE OrderTable;

IF OBJECT_ID('ProductTable', 'U') IS NOT NULL
DROP TABLE ProductTable;

IF OBJECT_ID('DiscountTable', 'U') IS NOT NULL
DROP TABLE DiscountTable;

IF OBJECT_ID('UserTable', 'U') IS NOT NULL
DROP TABLE UserTable;

IF OBJECT_ID('AddressTable', 'U') IS NOT NULL
DROP TABLE AddressTable;

IF OBJECT_ID('AdminTable', 'U') IS NOT NULL
DROP TABLE AdminTable;


Create table AdminTable
(
	adminId uniqueidentifier not null,
	adminName varchar(20) not null,
	adminSurname varchar(20) not null,
	adminEmail varchar(50) not null,
	adminPassword varchar(512) not null,
	adminPhoneNumber varchar(20) not null,
	adminSalt varchar(256) NOT NULL,
	Constraint PK_Admin Primary key (adminId)
)


Create table AddressTable
(
	addressId uniqueidentifier not null,
	street varchar(30) not null,
	streetNumber varchar(20) not null,
	town varchar(20) not null,
	country varchar(100) not null,
	Constraint PK_Address Primary key (addressId)
)

Create table UserTable
(
	userId uniqueidentifier not null,
	name varchar(20) not null,
	surname varchar(20) not null,
	email varchar(50) not null,
	password varchar(512) not null,
	dateOfBirth date not null,
	phoneNumber varchar(20) not null,
	addressId uniqueidentifier null,
	salt varchar(256) NOT NULL,
	Constraint PK_User Primary key (userId),
	Constraint FK_User_Address Foreign key(addressId) references AddressTable(addressId) ON DELETE SET NULL
)

Create table DiscountTable
(
	discountId uniqueidentifier not null,
	discountType varchar(20) not null,
	percentage varchar(20) not null,
	Constraint PK_Discount Primary key (discountId),
	Constraint CHK_Type check (discountType in ('13-18','18-30','30-45','45-60','60+','All')),
	Constraint CHK_Percentage check (percentage in ('10%','20%','30%','40%','50%'))
)

Create table ProductTable
(
	productId uniqueidentifier not null,
	productName varchar(20) not null,
	description varchar(100) not null,
	image varchar(100) not null,
	brand varchar(20) null,
	category varchar(20) not null,
	size varchar(10) null,
	price float not null,
	totalRating float null,
	quantity int not null,
	numOfReviews int not null,
	adminId uniqueidentifier not null,
	discountId uniqueidentifier null,
	Constraint PK_Product Primary key (productId),
	Constraint CHK_Category check (category in ('SeasonTicket','Jersey','Shorts','T-shirt','Cap','Hoody')),
	Constraint CHK_Size check (size in ('10','12','XS','S','M','L','XL','XXL','XXXL')),
	Constraint CHK_Price check (price > 0),
	Constraint CHK_Quantity check (quantity >= 0),
	Constraint CHK_Review check (numOfReviews >= 0),
	Constraint FK_Product_Admin Foreign key (adminId) references AdminTable(adminId) ON DELETE CASCADE,
	Constraint FK_Product_Discount Foreign key (discountId) references DiscountTable(discountId) ON DELETE SET NULL


)



Create table OrderTable
(
	orderId uniqueidentifier not null,
	numberOfItems int not null,
	orderStatus varchar(10) not null,
	orderDate date not null,
	totalPrice float not null,
	isPaid bit not null,
	userId uniqueidentifier not null,
	Constraint PK_Order Primary key (orderId),
	Constraint CHK_OrderStatus check (orderStatus in ('Pending','Sent','Delivered')),
	Constraint CHK_TotalPrice check (totalPrice > 0),
	Constraint FK_Order_User Foreign key (userId) references UserTable(userId) ON DELETE CASCADE

)

Create table ProductOrderTable
(
	orderId uniqueidentifier not null,
	productId uniqueidentifier not null,
	amount int not null,
	Constraint PK_ProductOrder Primary key (orderId,productId),
	Constraint CHK_Amount check (amount > 0),
	Constraint FK_ProductOrder_Order Foreign key (orderId) references OrderTable(orderId) ON DELETE CASCADE,
	Constraint FK_ProductOrder_Product Foreign key (productId) references ProductTable(productId) ON DELETE CASCADE
)

Create table ReviewTable
(
	reviewId uniqueidentifier not null,
	reviewText varchar(100) not null,
	rating int not null,
	userId uniqueidentifier not null,
	productId uniqueidentifier not null,
	Constraint PK_Rating Primary key (reviewId),
	Constraint CHK_Rating check (rating in (1,2,3,4,5)),
	Constraint FK_Review_User Foreign key (userId) references UserTable(userId) ON DELETE CASCADE,
	Constraint FK_Review_Product Foreign key (productId) references ProductTable(productId) ON DELETE CASCADE,

)

IF OBJECT_ID('RatingChange', 'TR') IS NOT NULL
    DROP TRIGGER RatingChange;
GO
CREATE TRIGGER RatingChange
ON ReviewTable
AFTER DELETE, INSERT 
AS
BEGIN
    IF @@ROWCOUNT = 0 RETURN; 
    SET NOCOUNT ON;

    DECLARE @productId uniqueidentifier, @rating FLOAT, @action VARCHAR(10);
    DECLARE rating_cursor CURSOR FOR
    SELECT productId, rating, 'INSERTED' AS action FROM INSERTED
    UNION ALL
    SELECT productId, rating, 'DELETED' AS action FROM DELETED;

    OPEN rating_cursor;
    FETCH NEXT FROM rating_cursor INTO @productId, @rating, @action;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @action = 'INSERTED'
        BEGIN 
            UPDATE ProductTable
            SET totalRating = (totalRating * numOfReviews + @rating) / (numOfReviews + 1),
                numOfReviews = numOfReviews + 1
            WHERE productId = @productId;
        END
        ELSE IF @action = 'DELETED'
        BEGIN
            DECLARE @numOfReviews INT;
            SELECT @numOfReviews = numOfReviews FROM ProductTable WHERE productId = @productId;

            IF @numOfReviews != 1
            BEGIN
                UPDATE ProductTable
                SET totalRating = (totalRating * @numOfReviews - @rating) / (@numOfReviews - 1),
                    numOfReviews = @numOfReviews - 1
                WHERE productId = @productId;
            END
            ELSE
            BEGIN
                UPDATE ProductTable
                SET totalRating = 0,
                    numOfReviews = 0
                WHERE productId = @productId;
            END
        END

        FETCH NEXT FROM rating_cursor INTO @productId, @rating, @action;
    END

    CLOSE rating_cursor;
    DEALLOCATE rating_cursor;
END
GO



-- AdminTable
INSERT INTO AdminTable (adminId, adminName, adminSurname, adminEmail, adminPassword, adminPhoneNumber, adminSalt)
VALUES
    ('1dc57b54-af65-4e41-8a8e-57d8854b5257', 'Marko', 'Petrovi?', 'markopetrovic@example.com', '6cYOOSKN3j2620SwiP7az23Wyc6MGvxSdwml2yhvahNX/23mD/lpLgxUjrYJvlRJyjjX/4ZDOzF+YoHvMx6qqkqK5pRQqEW2VGYV9Jr5/riEKVoM3OjYPi5oFwG4G44g7j3G+5vLorphJ2efCbNp0g2mFBoGWTbBjxK1dOuqpttRus0ad/Ktp+NdWL2A+YstEmJ7L2mKiMAqa3WurJP7zM2sxZIaY0XRYyrEr76roxmGQmwyACERejj45zkpZ1/PsliOcfK5edE1Pk8sv1E4yvqTYN5WHHcp3CoETQ9hpDn+QW7lxpv/lxJdNBTDLdeYvsqWAU8gWlGi4htR4lmdTg==', '1234567890','cIAu8MwSIdw='),
    ('fcf652d4-f1b1-495f-a363-26f5ce2d7124', 'Ana', 'Ivanovi?', 'anaivanovic@example.com', 'RhNDkvT5APmeN8rknWQuHge17cSNAANIcsmxsnDTx9CtonyTSQVTDy+Hlgj9PYDOvZTmozhmkTcBznl1SnmPIWDIfienBPDaik4JlLaqOMCmrGVplSqG1iLgc4qN+w/fyz7WXwTfr6Yla8Xy05idVEpEUtUIryFmup2elwq25TKOQo4zo7BI7eTKyjiX1/gks+6gPH3XKvyT16mEgJwj5rplDnh6284SaaVxLL8kU1B4hg4xTA6eVBiUX/oyAyBbz9oCQnbfcEZ2RtWS/mnSuWDd0/C/yQe3UuEGX4DnurwW8esoCG7Q+9e6yIEuSS8cTO0BZLWwe6f4xll81eTxcQ==', '2345678901','ThRUGutFhoKqHf1O9RL5dw=='),
    ('7be16be2-f4a3-4b26-ae86-1ef034064335', 'Milan', 'Jankovi?', 'milanjankovic@example.com', 'nXKKC+F2f8fgSezaVx4Vy6Hiau4vRCp/dT66US0G2NoJl0Yn0X8Kg0jpGbvPTDSfEvlC+RChXrjwb8JFYEEQBXWQ1KtosWfsdt0iV6DG4HZ77clTi9aSG0wa+jHGzCL0M2VRjW5ve3ilzGSADCUmt1iKaddW9Rd4Z7n26X7TumyztLDgHzC/letVn3NkLNaJ13PowKqyqhC+tyihpd40DtDNOfkWJJPzrrK6AeOM+wzm0whWNz7ZkvzKAg87dlXDOJaUe2aZJFN4gnnHanr+1ecUvkdBOyscyhK0sxrN9Lo0HtHYXub8H7eKpeMVeTakpIlv6kN9k+nBSJcU7TzY9w==', '3456789012','5VeLdNszXiBgrKp6/8+9sg=='),
    ('1f1f67d6-2093-4e53-99b5-b9b13cc5b4e3', 'Jovana', '?or?evi?', 'jovanadjordjevic@example.com', 'xunxGrvPYnw9ttbkEF8benyYzqK1W7M+PnpJfDzDF2CK4e1WDfQEx4IFIF8a5+baUbQFrUjsFvS3Z6rh8bKCV2Y5Y3YExZsS4HW+TTXmEQ/Q/wY6CABJww83BIb31eaeBiF/RAEgiV7q0o+lefRMm9GMNFnWpg8cdssIhrIbwcNZxPcrgk+GAQLAAwjypKMQcWfq3XmnzbtRjAN18JUyAunbufFH8Udbif10Fen+3nolQHL08oMvUSNuxv2Lt5axnBmePFsJfYzpAVDkXkNg5sfs60lRi31nXxZIG/+2JW3Z9USsg2FxzmyGl4Lto7hudHmUptCTQRLwwWg1al7Q4w==', '4567890123','thLVK2mPkGbgVS5rrhh2Pw=='),
    ('bf26dc0f-5e04-4955-997e-87b2c681c2d4', 'Stefan', 'Nikoli?', 'stefannikolic@example.com', 'OVAZtOZTH8iTT0ScFOfPTQpWC/cbgXIdz9xbUmq68RlYE8b0954gYmv/90AVAmDhxbpSShjUHsbLfxB35hNm5brexG6QyvoIOjVhUz7VAjpDYI4e0nJREEv2LLx5FxhvRU3mUhXY4iNkUAB22R4cCDQYFwcZmEebk1HnVt26e9if1JmZ4snbVkewHZflMwY6cJMXO0Mcnz+t05gGbLyfIGaptamn063qCi4CvudjhMlBTnGLBUm9aLGsZ1yDYt56jL2k0U/EVkUapE/yAXvAjoChv7b/DmnldmAAIRfpzOiUSpfxu9IKfvCfdSxf9E1s0IGHz7Y9hRr8H4uT4M86og==', '5678901234','+mi35MGht2ZrCPxFQ9XKdA=='),
    ('bcb20641-aa05-47aa-9c62-3f30bc32f800', 'Ivana', 'Pavlovi?', 'ivanapavlovic@example.com', 'cvIsglu3y1V5zSiTrKfaYbpbW57yC2jQKym8w8duBQw8BKzIhEPZERb03myaCdSGna5pNw27rZ4LVDT/YbERsKrxQq1bh8tSdyje2peopTfgAZvQtbuUN/YP9sjKwGe17O182p4Ngj6xyxh859w4k3Yx3eBmXuMBgLb+fx1yA+Wlz2G0w4a1mBVCh7W6sRbNVNSFQG/40jsiEFKhY/zG1/CIeORMXrzdQ8Wu68BdKn5J9Z4gR07IP057Aa80JccDA5cqfzCUevmgbAHOuYNTfJeROdBgnlTIYXUIL90xR33VW58thruld6LwlMrhFKEZE9YChpYr6Iz2CGXwcRn30A==', '6789012345','7KcvaiyP1WMj1ifkPfBxwA=='),
    ('fd0e7bcb-25f0-44af-8493-c65fe1e648cf', 'Nikola', 'Markovi?', 'nikolamarkovic@example.com', 'lfhnbKbkEvpNxvuN94DIEU6Ha8taOWNiMtU+0V1dcmFF2FcyRXab+Px6EsRrYEPJEix1fcvurdzcJKowPLoFrGryJId6PAioDDbXqewTrUec4ZMkLBSipOQwF+ru/lavi2qRIy6ynnCyfJgdxSO0VxnzHV7+Qm40KgHea20urpfi/n9cWGc2UtxzaLyCM5fJdocbyyfpta5wA18P1oJ8rdW57ha5mIg5nG6bGbVJFJm6CFtCwsEg4EHgkHzDmvJVDsUF2se6QY6qE3MEbx7vH63p9NdgryHk3s2bfqK8XO84Xail2/c0hWw6NMwEg+x0U25tWJQDSPyLhWh2b0SVPQ==', '7890123456','CEMhBk4yWGCm84WLdWOT7w=='),
    ('2a7ce048-4d4e-4e46-9d5c-5a4ff9ff74d7', 'Jelena', 'Stojanovi?', 'jelenastojanovic@example.com', 'Os/PvQw0xyzRbJhcpzxkyK0n3OBuidrwiwnOBMqpAQIEvtdOnYjTyEML6Rn2jaf+x6nMFcUcPsVmMLC7q3qbU9gcCcHpxFIQ9NfDTHPr8aj0mrJ3yoHhGmA82lzYj6rP14zLLFmMY8TXJdR5jq7DdY5hhDVln1p5QzjeYHSX7QYHVlOFuRII+mApm64iLLpv7Oqf03uittSolVrOyVqnoaznENSy/g6OOVEWxeDcT3/4Jay6KaTKJqhGXod4kzDPFpFgDchflmmu0q9XdvIYEZ1EKSS0rBCkFL3tRzvZHml9oCqeUH6iZbo+9GbEg94NL/nA1uuF2uTpQy4MvzaIuQ==', '8901234567','bnJjbFznjozvE3gpaYLCdQ=='),
    ('f18ec27d-1e36-4f90-bd1c-4f09be95cf77', 'Vladimir', 'Kova?evi?', 'vladimirkovacevic@example.com', 'bVpdIynrSIHqpUtR3fkgTw+fuQjiDvwILqkkA/69xTt6QSgLqBM6QjfpD3t0hqxSc7sicXGR6Sng3Uip7XtJ4Rmp7DnHPDXyq4b8+gRMnc9B7OE0F6i0XLnH78KZgHnPZWV3h3LotGOC76e5M/4hLsjriDEtkE2PsNilrgS13Ozx0f4GAA+Y9CTfZ20FLAiu8rSNerFKPnzNMzMoBJ7UsYjgs+xOGdsL/8SIToig1/aQ2BzE/YwLZRaoWtcpvx8GfRZkwvYUJJjnIT3CZAlA5DCBj7MEr89BmzcAIvTWKjIHi6Dn51/Qks1FNLVvMmCcdrxivgDrqst3PdDil6W5dw==', '9012345678','2ujdhs7hYHg2m2mrJVge1w=='),
    ('f2b6e5e6-b3f1-4f67-8610-77d2c32b4ee8', 'Tamara', '?or?evi?', 'tamaradjordjevic@example.com', 'm9XcQEbvziRo5l/UAdyR7gcBl/rVrT/UX/Yika/pk718XYloViPbPZnrzc2rmt0ax9WSFnJkYRWEdV+SY2TdYajcaWhfimvKClOMAD4HWuMf3iGbHFHMCIP1yo2A8tIgYMpgHw8so2yTevzEl8toRNkMjYql8c+8uAfdYFl9sr91OsDvxlsUHDep7BA1TAuq+Hq5poB9f4Mw//3qfwJhXxzWeh8tp7+iFDDF2d+rbbB36laK40/FamVrq6oQRXyoiXs+Tq3ZWk24KyCh4sJbmGWfdkzAbeYOKPJtqJJlV53tgg3RF6g11kYl65kqoQti1UIE/EClsmphjlAqOY2urQ==', '0123456789','CYRgFQnmewjpEF++vvRwKQ==');

-- AddressTable
INSERT INTO AddressTable (addressId, street, streetNumber, town, country)
VALUES
    ('6b74a8eb-69c0-4d2d-8465-3dd3a7b76d3b', 'Bulevar Kralja Aleksandra', '123', 'Beograd', 'Srbija'),
    ('02c8f0d6-b7e3-45d4-a4ed-07d5dd46fbc2', 'Kara?or?eva', '45', 'Novi Sad', 'Srbija'),
    ('de542037-c77c-4a2d-94a5-c8a91c51ce7c', 'Kralja Milana', '8', 'Niš', 'Srbija'),
    ('b0473d21-6596-4fbf-b0a2-fdc3ecb442e4', 'Vuka Karadži?a', '21', 'Kragujevac', 'Srbija'),
    ('c8a3dd80-06b3-4488-a6d7-73f69dc4ab8f', 'Nemanjina', '15', 'Subotica', 'Srbija'),
    ('7480a4e6-1d45-46c0-bc17-1ae7eb7be724', 'Svetog Save', '3', '?a?ak', 'Srbija'),
    ('e0634405-33c3-4b23-8c3a-4c3c088dfe27', 'Kneza Miloša', '33', 'Kraljevo', 'Srbija'),
    ('1dd3c0e8-8da5-45f5-b057-5f3ac72f607c', 'Nikole Paši?a', '7', 'Zrenjanin', 'Srbija'),
    ('66da5c29-0d1c-4b6f-8a01-5e49613920db', 'Jovana Cviji?a', '12', 'Pan?evo', 'Srbija'),
    ('152e5e1d-1ec7-4b67-9b56-f10f7173e96e', 'Kara?or?eva', '17', 'Novi Pazar', 'Srbija');

-- UserTable
INSERT INTO UserTable (userId, name, surname, email, password, dateOfBirth, phoneNumber, addressId,salt)
VALUES
    ('6539c162-b1a7-4544-a62b-7e0db3734d1e', 'Jovan', 'Stankovi?', 'jovanstankovic@example.com', 'vD3nIbqddv/gZUIG9ulDSLN1pKOKaKcq6rA1opznSg/6ydGmPS6kjJYsA2f/VaZK8oFB/WE8+SChicgPTMkZLom6p8YOZ4qWbx2MpmAJ7a9WhBdIOoA6zmLlZJTu+YuYxpa4jMf1u48jv9PvVicne7QerXoXMiWJ+NXvq5P8bt2fVi59Oq0FAZ8hCfNg+HZniZCM2R05LmMlxhIdnYnlPFo6v4NOhZIUdlRbfYQs1aQNGbCeHMuVOtQIFdtEcAZ7yfB0TLJqlMj6lz1gQdo9T0Ymk0s8SiUCudJgCWzGR7Y8OBv7GP5V0uNFGhqgjyUfUsaocit8b0RFAf0LRT9eaQ==', '1995-02-28', '2345678901', '6b74a8eb-69c0-4d2d-8465-3dd3a7b76d3b','2JHjkZ8oy3w='),
    ('1916fe33-14f6-4e24-9a3f-d02d0c62f698', 'Milica', '?or?evi?', 'milicadjordjevic@example.com', 'SFzqJkTCp8QPEc0ie+2eI3L3Wu8yM7ejM9MCuNtlz6nbdRL6zHx0ZTpzntCfmzpvWtac/xWoyAdwaaj9Fn9EzmBBtsWMPT8M8xVWIyE6Yei0UjggF1cyER/vX0pbcCUHIvj7sTN0KM8cSuPBHCelNUPQTGSqBsA5kk/BGZXOR1DwmYi14a9iwC81aGEh2B9pSgZTZpxM3ZhMWjFFLm5xI3QAOkexwDF4HPOWzL18U8T/ThrLpP3StLQLf0inODQeJrIsLJPpNnbfwX7+o4XCDQ0RT+ys4w17cFoDGzStxsdioKlJhCbqhVTyFSrG+QiirHeGvUPBC85fSyJ0FF+H2Q==', '1980-07-15', '3456789012', '02c8f0d6-b7e3-45d4-a4ed-07d5dd46fbc2','MfW34fVHjzblRgeJ6gdsZQ=='),
    ('7be0dd99-0e8d-45f4-aa16-8a6b3c5b8c61', 'Nenad', 'Jovanovi?', 'nenadjovanovic@example.com', 'FsQsuTpnJM7aZE7PTusK2n9wcOVzfdJEgbUwO+Y//XNZPleEtB+xbKgn4tO4em3KndnjWSTu+zSgjXGrfhrdR+6wrWy8sYQRI115mpU5R8ms/mEayObKLYjcvg6oCMgRIzLIn+SfY3tNuH2PX3ZfJwNEuUPe3SRXIY8I85a6C3hybvJWXKK+FpKlZf8KrymDxBjY3WR7EkXQCM4Po6i1fu0nbW0UaQ85RSJHFVpweWTy8S/HNO4r5AZrVeecQc23ZFm32rAuC4uLsZrUxQUEjtwsnS7MxoU1g6VoT7Ce7CJqjOpy4/ecgL2viHG8eIjvkaG4QFUEuo+psH9RsdhJhQ==', '1972-11-10', '4567890123', 'de542037-c77c-4a2d-94a5-c8a91c51ce7c','at3AJLPJ+qiMPJ53RwzuIA=='),
    ('93fd0b0a-ea2e-43f6-baa5-7f74c6a77868', 'Ana', 'Petrovi?', 'anapetrovic@example.com', 'XPCulYKD6e5N7of1qdrE3Wc+n2J2bMA3KTdbfGDjMEoR5cbqZre3Rq67bvml/KWNFPh2/0leOG0Cshle2FcRwmW4ZN0Hnwwife5wX1v2yP/y5p5hAlt92Vw+UlYKhlhNuzcs9DMHjmkbcIMOe6vtpM4FkpaHCw1OYE2Udnl+zouiQv0+nJhEBZ+XykSVPicgUYA7tJhZouqe0TF/YAc7Zaq5LNiBkpX+XI+lkyqaNLYHxeQiJ43T9VRxHXM3ifR6tkeEZfLBpZP7LQ8ztpR6ruwQJY2R1RU+z8HnpZQN+12M1l7oW55+hSmap32aljkXnRbc3aHxNPiBgd88VHhnLQ==', '1963-04-25', '5678901234', 'b0473d21-6596-4fbf-b0a2-fdc3ecb442e4','I4VXaxVyMrr8cFnR8eYUtw=='),
    ('2beaf6a3-7dc0-4c3f-b4cb-4c8dd077c5d7', 'Nikola', 'Ili?', 'nikolailic@example.com', 'TJmnOowgeFgteFUVKgUpDtdZGEwwRdVfDMz+40DMarzDh7IVma5YKzIdeuaIvGsRuWVglSwkGNRjiaD0L1fweKSoYw58AJHGhrl+0OD6mo6QBBJ0hdzYSKc2e1YjdRvkMYfi0a/vl2o6HP+xucuVxvCvLrMv1h/V/YJ+LS4VUDGsqXW6a8NVQyA30bBsLncipoW9aYKJ0R5B16Efotvrx7kLN7vOFLwemKFxDlxFG7O1net1CC3mfkaXx344scL84x6KU30/WYQzR2rMrHHwFiMdSoyBbfNB0vGhhJKK2wUKmaJb959O0MCsnJbEfi9Xyq5b38F1NBjLKZAPK3gqGg==', '1950-09-20', '6789012345', 'c8a3dd80-06b3-4488-a6d7-73f69dc4ab8f','p9i+u+B6smPzZoDDBZd0nA=='),
    ('c9a26a88-9a5e-47f2-af36-85b90ec0ff12', 'Jelena', 'Kova?evi?', 'jelenakovacevic@example.com', 'kQ+qq+FWx7elvAviCrjvWzQDuvkNsTMVTZljiXAlfkVGxkzP2TxQ1OVaOZfKM8s9jO3JSa8IwRw85lknc0GxfBwoZyNs3OINaoAXqOldwV7ekJOCaMkTGOzmQvnbgJOippXmq+z8O+lPLT6JORkqn9Z78FTh7Fyz4XInuDYKRBqN+9nrLzE9PmYtZl/B/cps2N2Rq2zNegCmc4KnvU9bh4m0yViUkv6EM+ILfTKqs9B4mh7DRz4K+mgXpBTXl987jPEYePJJTme56FfwG5RYK7Pa+vHNeGgc2DOBQIJaoqzlp5TfjpxMZbPC3YegqKiMovHMp5tDtcHRcyZDjGoKqw==', '1940-01-05', '7890123456', '7480a4e6-1d45-46c0-bc17-1ae7eb7be724','LN88EE/k0U8Z8JDTBueLUA=='),
    ('92b512a2-9e0a-4e09-ba42-9606a92a5fd8', 'Stefan', 'Markovi?', 'stefanmarkovic@example.com', 'gCXrwt4kNq/ZeWNPug5XNWXf+GtlextL1JGz6xcdrZIxgTLfrqcx7O2tevMvqbhp/jgr9RepMKPTU/reouFVWLGZyWtdXrw6Lsu+32cUoSD77T90WiS+docgohu8EpDW6RaNa8qihJuSUITcl4iNGptKIzbPyCXguiglykZC62ONyafGJ04woyJgGRlfrnSp9hrqj3Ke/2LVXIM6Irzroz29vtdocK6I1xoiPEpwgGEKd1oGHlL6pNXY68n91/oqjG3AZWeWE+hTAyS+Smu07zdRzzzMfXIbGG9eygl+31n13cL1ZjCBN+oisFVl3m/ZixP77uU0tH6bJRmvfBzZ9w==', '1930-06-12', '8901234567', 'e0634405-33c3-4b23-8c3a-4c3c088dfe27','Gs7uZAnIQc3fkjOf2ajMDQ=='),
    ('289bcb80-f5f2-45ee-935b-7589466544a6', 'Maja', 'Stojanovi?', 'majastojanovic@example.com', 'SOEickU8GyCo1hgUfB+5zhLmQAdPfGm78BtVN3K4wKMlzOCCP2BfSYCSkR0zjV4yPw0c90g0g+qMhelodu9IY/cAbPsQ1VftIZsTmMS2oSgh3aQlCqoiWdTjNV6wCxhB2NlsQzVvTPFznN4XplOz+pnQFwOAuv8NZgLe0WmZJdxABIin1Qf0W3T7U1N/FqVYCLyDfzAW2TTLxwELc6lHgogYjcOAq3/dHZ4jLx6mC1IFJsuxbgN+tlagLcX3ueefgKgfR38iPwQCdjwYa/HUpTaYpM8iI74B8KVAPZyt0A42JM73oadgtPf5yVo4epn9IO3BaRrVxl6o0sbBP8RHxA==', '1920-08-18', '9012345678', '1dd3c0e8-8da5-45f5-b057-5f3ac72f607c','MZKphd2aZHjwvfoMijse7Q=='),
    ('9b8f3a87-dae0-4a29-8e4a-19f1dc07da6c', 'Petar', 'Pavlovi?', 'petarpavlovic@example.com', 'mZRa5797Kh/NS9qYpN32Wuc6h9vgpqdjEc8uBSxbjZ3jvEUmO9fxrKI6digvFcbVXFQG/HlZ+m7XilI9xfEcERDjdCDwYyeBdtx/JkQY6T8s6DD8E4kSRsQUz1AuFCXqjanCwIo5+fy5MXMCOwvzlBJ4GoXiHnDkeR7YzMP/4lreNh4P/KDRXKv8tD8E1b8FS/KZ2rzBwgL0achy5mqElB9iGYN6VlCLoIdtDJ6VThjIx9EdHIGS35GFlAeOSt18Es7R2njj3/0+jJUjPh1kG36XlDtKaXnSHu19710JI4A263B86jMKhYf/9N9hO1hu5gnSQXLWqfOvMXJ0iS5hkA==', '1910-10-23', '0123456789', '66da5c29-0d1c-4b6f-8a01-5e49613920db','K2hBzPJTO2yzTBptSSPjXg=='),
    ('f0c48e35-4cb8-4a53-b464-269f7a8a8201', 'Jovana', 'Nikoli?', 'jovananikolic@example.com', 'bYJniKGfJmaVpMs5KdepITgv5YCloZOKZMX1Eb8Oe8gi8XZymzxhBDRTygLDAaYkJG9cHZLJ9jCs+INXiIQgpoEuDdoF5dCZ2luUKwFFWu8HJRnhCg6mFVXgTpyBYCBwarNeOK76OH2sN72rOX8bcXAXP/8EOZb4AjewH9/u0nFPud4Ho7UdiHqBLfbFzDEbkCKxXJVtO5bKhvbejaDlNp0WAuf+i/MkDi2NRuQPzduf4fk/t+zIVy10nEXxKoMvgd0NIfuiVBCSFQ9ZXxPcLxyCmsQnEQYq1kedHwBWqPLZgB+oCZGPcANl0NHiz8gCVwlx0qRWzS3ZghelVz60Gg==', '1900-12-30', '1234567890', '152e5e1d-1ec7-4b67-9b56-f10f7173e96e','C9VtAaN95fG01/XXD2FFtQ==');

-- DiscountTable
INSERT INTO DiscountTable (discountId, discountType, percentage)
VALUES
    ('875c3508-5296-4124-83bb-d4e8e1249bc1', '13-18', '10%'),
    ('5fdeae0e-b999-4c53-bc4b-3f7ee7e3d2f8', '18-30', '20%'),
    ('3e24f35d-785f-4e0d-906b-056ad20e1d86', '30-45', '30%'),
    ('c3510980-0e02-43ee-905f-f52a1ddc8d8e', '45-60', '40%'),
    ('ae6e011f-15d3-475f-9c62-f75282a3a592', '60+', '50%'),
    ('f74c2c86-cd24-48d8-b26b-262272f85825', 'All', '10%'),
    ('a5d3345a-f6b0-4a29-9318-8b74aa669d77', 'All', '20%'),
    ('6e0f8950-3c0a-4541-aab5-b0a6e7735b45', 'All', '30%'),
    ('e11cf23e-45b8-4f76-9bb8-12619e9f4c0a', 'All', '40%'),
    ('a2636dcf-2f9b-4ec8-9951-0e9b4d7861f8', 'All', '50%');

-- ProductTable
INSERT INTO ProductTable (productId, productName, description, image, brand, category, size, price, totalRating, quantity, numOfReviews, adminId, discountId)
VALUES
    ('0dd5b96d-4a6f-48f8-94d4-55b27a8f4658', 'Sezonska Karta', 'Kompletna sezonska karta za sve doma?e utakmice', 'assets\images\season.png', 'Klub', 'SeasonTicket', NULL, 100.00, 0, 100, 0, '1dc57b54-af65-4e41-8a8e-57d8854b5257', '875c3508-5296-4124-83bb-d4e8e1249bc1'),
    ('5a58663c-54c2-4d26-ae57-4b892d27e3e2', 'Dres Doma?i', 'Dres za ku?ne utakmice', 'assets\images\jersey.jpg', 'Nike', 'Jersey', 'L', 80.00, 0, 200, 0, 'fcf652d4-f1b1-495f-a363-26f5ce2d7124', '5fdeae0e-b999-4c53-bc4b-3f7ee7e3d2f8'),
    ('a8ff11ec-878e-4d36-8375-eeff43d74659', 'Dres Gostuju?i', 'Dres za gostuju?e utakmice', 'assets\images\awayjersey.jpg', 'Adidas', 'Jersey', 'M', 80.00, 0, 150, 0, '7be16be2-f4a3-4b26-ae86-1ef034064335', '3e24f35d-785f-4e0d-906b-056ad20e1d86'),
    ('29f1e729-3bc4-4b74-a724-6e4cb8ff9ebf', 'Šorts', 'Šorts za treninge i utakmice', 'assets\images\shorts.jpg', 'Puma', 'Shorts', 'M', 40.00, 0, 300, 0, '1f1f67d6-2093-4e53-99b5-b9b13cc5b4e3', 'c3510980-0e02-43ee-905f-f52a1ddc8d8e'),
    ('57967b07-b1f1-4d3a-b8e3-5a634ac1a6db', 'Navija?ka majica', 'Majica za navija?e kluba', 'assets\images\fanshirt.jpg', 'Under Armour', 'T-Shirt', 'S', 25.00, 0, 500, 0, 'bf26dc0f-5e04-4955-997e-87b2c681c2d4', 'ae6e011f-15d3-475f-9c62-f75282a3a592'),
    ('67e7b94c-c0a6-48fc-b713-54d6b158276e', 'Ka?ket', 'Ka?ket sa logom kluba', 'assets\images\cap.jpg', 'New Era', 'Cap', NULL, 20.00, 0, 400, 0, 'bcb20641-aa05-47aa-9c62-3f30bc32f800', 'f74c2c86-cd24-48d8-b26b-262272f85825'),
    ('81d2a127-cce4-4e5f-98a5-aa62e221fb7c', 'Duks', 'Duks sa kapulja?om', 'assets\images\hoodie.jpg', 'Champion', 'Hoody', 'XL', 60.00, 0, 250, 0, 'fd0e7bcb-25f0-44af-8493-c65fe1e648cf', 'a5d3345a-f6b0-4a29-9318-8b74aa669d77'),
    ('775ef3a6-4a7d-4be2-b7c0-f2e628775667', 'Majica', 'Majica za treninge i utakmice', 'assets\images\shirt.jpg', 'Nike', 'T-Shirt', 'L', 10.00, 0, 600, 0, '2a7ce048-4d4e-4e46-9d5c-5a4ff9ff74d7', '6e0f8950-3c0a-4541-aab5-b0a6e7735b45');

-- OrderTable
INSERT INTO OrderTable (orderId, numberOfItems, orderStatus, orderDate, totalPrice,isPaid, userId)
VALUES
    ('1c58bace-30ac-4d9a-a896-7ba044451367', 3, 'Delivered', '2024-03-15', 160.00,1, '6539c162-b1a7-4544-a62b-7e0db3734d1e'),
    ('3a7580e8-2a53-4c1b-af99-56b0b5a5d7d3', 2, 'Sent', '2024-03-15', 160.00,1 ,'1916fe33-14f6-4e24-9a3f-d02d0c62f698'),
    ('6f7b5d5e-8b51-4f24-a4ba-755529d7479e', 1, 'Pending', '2024-03-15', 40.00,0, '7be0dd99-0e8d-45f4-aa16-8a6b3c5b8c61'),
    ('0c413c1a-645f-4b42-b359-83b0ab080004', 4, 'Pending', '2024-03-15', 100.00,0, '93fd0b0a-ea2e-43f6-baa5-7f74c6a77868'),
    ('9d3eb2c5-1e58-48fc-ba06-5cb4308d9143', 2, 'Delivered', '2024-03-15', 50.00,1, '2beaf6a3-7dc0-4c3f-b4cb-4c8dd077c5d7'),
    ('61c819a6-2c7c-4cfc-93d1-6a438f405868', 1, 'Delivered', '2024-03-15', 25.00,1, 'c9a26a88-9a5e-47f2-af36-85b90ec0ff12'),
    ('6c2be3a4-d235-45e8-9a8c-41fb53e557c2', 3, 'Sent', '2024-03-15', 60.00,1, '92b512a2-9e0a-4e09-ba42-9606a92a5fd8'),
    ('79375da3-c05c-4298-b0ae-f1e89c0ffea3', 2, 'Pending', '2024-03-15', 20.00,0, '289bcb80-f5f2-45ee-935b-7589466544a6'),
    ('4a776f80-84cc-4e27-bcc7-0ec6478cf24a', 1, 'Pending', '2024-03-15', 30.00,0, '9b8f3a87-dae0-4a29-8e4a-19f1dc07da6c'),
    ('7ee7df90-b0f7-407f-b3e4-b19086755f97', 5, 'Delivered', '2024-03-15', 250.00,1, 'f0c48e35-4cb8-4a53-b464-269f7a8a8201');

-- ProductOrderTable
INSERT INTO ProductOrderTable (orderId, productId, amount)
VALUES
    ('1c58bace-30ac-4d9a-a896-7ba044451367', '0dd5b96d-4a6f-48f8-94d4-55b27a8f4658', 1),
    ('1c58bace-30ac-4d9a-a896-7ba044451367', '5a58663c-54c2-4d26-ae57-4b892d27e3e2', 1),
    ('1c58bace-30ac-4d9a-a896-7ba044451367', 'a8ff11ec-878e-4d36-8375-eeff43d74659', 1),
    ('3a7580e8-2a53-4c1b-af99-56b0b5a5d7d3', '29f1e729-3bc4-4b74-a724-6e4cb8ff9ebf', 1),
    ('3a7580e8-2a53-4c1b-af99-56b0b5a5d7d3', '57967b07-b1f1-4d3a-b8e3-5a634ac1a6db', 1),
    ('6f7b5d5e-8b51-4f24-a4ba-755529d7479e', '67e7b94c-c0a6-48fc-b713-54d6b158276e', 1),
    ('0c413c1a-645f-4b42-b359-83b0ab080004', '81d2a127-cce4-4e5f-98a5-aa62e221fb7c', 1),
    ('9d3eb2c5-1e58-48fc-ba06-5cb4308d9143', '775ef3a6-4a7d-4be2-b7c0-f2e628775667', 1),
    ('61c819a6-2c7c-4cfc-93d1-6a438f405868', '775ef3a6-4a7d-4be2-b7c0-f2e628775667', 1),
    ('6c2be3a4-d235-45e8-9a8c-41fb53e557c2', '0dd5b96d-4a6f-48f8-94d4-55b27a8f4658', 2),
    ('6c2be3a4-d235-45e8-9a8c-41fb53e557c2', 'a8ff11ec-878e-4d36-8375-eeff43d74659', 1),
    ('79375da3-c05c-4298-b0ae-f1e89c0ffea3', '67e7b94c-c0a6-48fc-b713-54d6b158276e', 1),
    ('4a776f80-84cc-4e27-bcc7-0ec6478cf24a', '67e7b94c-c0a6-48fc-b713-54d6b158276e', 1),
    ('7ee7df90-b0f7-407f-b3e4-b19086755f97', '0dd5b96d-4a6f-48f8-94d4-55b27a8f4658', 3),
    ('7ee7df90-b0f7-407f-b3e4-b19086755f97', 'a8ff11ec-878e-4d36-8375-eeff43d74659', 2);



	-- ReviewTable
INSERT INTO ReviewTable (reviewId, reviewText, rating, userId, productId)
VALUES
    ('d93093c4-2b6c-4a6e-9162-063b318b5cb0', 'Odli?na sezonska karta, pokriva sve doma?e utakmice.', 5, '6539c162-b1a7-4544-a62b-7e0db3734d1e', '0dd5b96d-4a6f-48f8-94d4-55b27a8f4658'),
    ('e90a0f29-f91d-4e76-ae67-1b03d835f787', 'Dobar dres, udoban za nošenje.', 4, '1916fe33-14f6-4e24-9a3f-d02d0c62f698', '5a58663c-54c2-4d26-ae57-4b892d27e3e2'),
    ('1d85d42f-9a95-4fa4-b162-d809336e4e27', 'Solidan šorts, dobra cena.', 3, '7be0dd99-0e8d-45f4-aa16-8a6b3c5b8c61', 'a8ff11ec-878e-4d36-8375-eeff43d74659'),
    ('e2df2276-6c5e-492f-a7fd-9f2b7ed6d890', 'Lopta se brzo istrošila, o?ekivao sam bolji kvalitet.', 2, '93fd0b0a-ea2e-43f6-baa5-7f74c6a77868', '29f1e729-3bc4-4b74-a724-6e4cb8ff9ebf'),
    ('bd1ba86d-bc5b-4a25-ba3b-4ba53ac00067', 'Majica je loše kvalitete, ne preporu?ujem.', 1, '2beaf6a3-7dc0-4c3f-b4cb-4c8dd077c5d7', '57967b07-b1f1-4d3a-b8e3-5a634ac1a6db'),
    ('2a285580-b1e3-415f-bbfc-af5ed6f1da72', 'Ka?ket je odli?an, kvalitetan materijal.', 5, 'c9a26a88-9a5e-47f2-af36-85b90ec0ff12', '67e7b94c-c0a6-48fc-b713-54d6b158276e'),
    ('2c8c2ae7-c7a1-45be-8e5f-c9a581fd1899', 'Duks je udoban i lepo stoji.', 4, '92b512a2-9e0a-4e09-ba42-9606a92a5fd8', '81d2a127-cce4-4e5f-98a5-aa62e221fb7c'),
    ('7393be56-77cc-4f71-bc1c-1d9f3ee4761e', 'Majice su previše uske, neudobne za nošenje.', 3, '289bcb80-f5f2-45ee-935b-7589466544a6', '775ef3a6-4a7d-4be2-b7c0-f2e628775667');
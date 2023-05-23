create table Categories
(
	Id bigint primary key identity,
	CategoryName varchar(255) not null
);

insert into Categories(CategoryName) values
	('Men'),
	('Women'),
	('Kids');

create table Products
(
	Id bigint primary key identity,
	ProductName varchar(255) not null,
	Description varchar(255) default null,
	Price decimal(18, 2) not null,
	CategoryId bigint not null

	foreign key(CategoryId) references Categories(Id)
);

insert into Products(ProductName, Description, Price, CategoryId)
	values
		('Puma Shoes', 'Men Puma shoes in black, white and red color',4500.00, 1),
		('Levis Jeans', 'Levis jeans for women',1299.00, 2),
		('Digital Watch', 'Watch for kids',2500.00, 3);
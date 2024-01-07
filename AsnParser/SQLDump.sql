CREATE TABLE Boxes
(
    Id uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    SupplierIdentifier VARCHAR(255),
    CartonBoxInIdentifier VARCHAR(255)
);

CREATE TABLE Contents
(
    Id SERIAL PRIMARY KEY,
    PoNumber VARCHAR(255),
    Isbn VARCHAR(255),
    Quantity INT,
    BoxId UUID REFERENCES Boxes(Id)
);
interface LegalForm {
    LegalFormAbbr?: string;
    LegalFormName?: string;
}
interface Address {
    CountryCode?: string;
    Country?: string;
    IsForeign?: string;
    DistrictEkatte?: string;
    District?: string;
    MunicipalityEkatte?: string;
    Municipality?: string;
    SettlementEKATTE?: string;
    Settlement?: string;
    Area?: string;
    AreaEkatte?: string;
    PostCode?: string;
    Street?: string;
    StreetNumber?: string;
    Block?: string;
    Floor?: string;
}

interface Seat {
    Address?: Address;
    Contacts?: any;
}
interface SeatForCorrespondence {
    CountryCode?: string;
    Country?: string;
    IsForeign?: string;
    DistrictEkatte?: string;
    District?: string;
    MunicipalityEkatte?: string;
    Municipality?: string;
    SettlementEKATTE?: string;
    Settlement?: string;
    Area?: string;
    AreaEkatte?: string;
    PostCode?: string;
    HousingEstate?: string;
    Street?: string;
    StreetNumber?: string;
    Floor?: string;
    Apartment?: string;
}
interface SubjectOfActivity {
    Subject?: string;
    IsBank?: string;
    IsInsurer?: string;
}

interface AddemptionOfTraderSeatChange {
    Address?: string;
    Contacts?: string;
}
interface Funds {
    Value?: string;
    Euro?: string;
}

interface Subject {
    Indent?: string;
    Name?: string;
    IndentType?: string;
}

interface Detail {
    FieldName?: string;
    FieldCode?: string;
    FieldOrder?: string;
    Subject?: Subject;
}
export interface RegixOrganization {
    Status?: string;
    UIC?: string;
    Company?: string;
    LegalForm?: LegalForm;
    Transliteration?: string;
    Seat?: Seat;
    SeatForCorrespondence?: SeatForCorrespondence;
    SubjectOfActivity?: SubjectOfActivity;
    SubjectOfActivityNKID?: any;
    AddemptionOfTraderSeatChange?: AddemptionOfTraderSeatChange;
    Shares?: any;
    MinimumAmount?: any;
    Funds?: Funds;
    DepositedFunds?: Funds;
    NonMonetaryDeposits?: any;
    BoardOfDirectorsMandate?: any;
    AdministrativeBoardMandate?: any;
    BoardOfManagersMandate?: any;
    BoardOfManagers2Mandate?: any;
    LeadingBoardMandate?: any;
    SupervisingBoardMandate?: any;
    SupervisingBoard2Mandate?: any;
    ControllingBoardMandate?: any;
    Details: {
        Detail?: [Detail];
    };
    DataValidForDate?: string | Date;
}

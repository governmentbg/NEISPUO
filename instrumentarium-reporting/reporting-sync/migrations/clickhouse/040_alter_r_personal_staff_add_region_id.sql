-- Migration: Alter R_Personal_Staff add RegionID
ALTER TABLE R_Personal_Staff
    ADD COLUMN IF NOT EXISTS `RegionID` Nullable(Int32)
    AFTER `Област`;



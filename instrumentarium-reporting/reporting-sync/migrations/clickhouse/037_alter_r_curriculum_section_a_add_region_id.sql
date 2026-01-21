-- Migration: Alter R_Curriculum_Section_A add RegionID
ALTER TABLE R_Curriculum_Section_A
    ADD COLUMN IF NOT EXISTS `RegionID` Nullable(Int32)
    AFTER `Област`;



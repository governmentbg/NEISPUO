-- Migration: Alter R_Curriculum_Section_B_Professional add RegionID
ALTER TABLE R_Curriculum_Section_B_Professional
    ADD COLUMN IF NOT EXISTS `RegionID` Nullable(Int32)
    AFTER `Област`;



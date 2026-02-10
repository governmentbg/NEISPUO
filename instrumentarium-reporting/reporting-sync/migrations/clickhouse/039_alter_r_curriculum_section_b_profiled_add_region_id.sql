-- Migration: Alter R_Curriculum_Section_B_Profiled add RegionID
ALTER TABLE R_Curriculum_Section_B_Profiled
    ADD COLUMN IF NOT EXISTS `RegionID` Nullable(Int32)
    AFTER `Област`;



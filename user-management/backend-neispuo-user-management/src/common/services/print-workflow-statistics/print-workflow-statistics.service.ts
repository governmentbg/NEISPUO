import { Logger } from '@nestjs/common';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { TelelinkCheckStatusResponseEnum } from 'src/common/constants/enum/telelink-check-status-response.enum';
import { TelelinkCreateEventResponseEnum } from 'src/common/constants/enum/telelink-create-event-response.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { VariablesService } from 'src/models/variables/routing/variables.service';

export class PrintWorkflowStatisticsService {
    static print(
        workflowType: WorkflowType,
        callerName,
        userRole: UserRoleType,
        workflows: {
            [key in TelelinkCheckStatusResponseEnum | TelelinkCreateEventResponseEnum]: any[];
        },
    ) {
        let top = VariablesService.CRON_JOB_TOP_VALUE;
        const userType = userRole || `ALL`;
        const workflowTypeString = workflowType || `ALL`;
        let buildString = `[${callerName}][TIME:${new Date().toISOString()}][WORKFLOW_TYPE:${workflowTypeString}][USERTYPE:${userType}]`;
        const workflowKeysToLoop = callerName.includes('check')
            ? Object.keys(TelelinkCheckStatusResponseEnum)
            : Object.keys(TelelinkCreateEventResponseEnum);
        if (workflowType !== null) {
            //workflowType is not passed on check methods because they contain all sort of workflows and are not just one type like USER_CREATE for instance.
            const topValues = {
                [WorkflowType.USER_CREATE]: VariablesService.CRON_JOB_TOP_USERS_CREATE_VALUE,
                [WorkflowType.USER_UPDATE]: VariablesService.CRON_JOB_TOP_USERS_UPDATE_VALUE,
                [WorkflowType.USER_DELETE]: VariablesService.CRON_JOB_TOP_USERS_DELETE_VALUE,
                [WorkflowType.CLASS_CREATE]: VariablesService.CRON_JOB_TOP_CLASSES_CREATE_VALUE,
                [WorkflowType.CLASS_UPDATE]: VariablesService.CRON_JOB_TOP_CLASSES_UPDATE_VALUE,
                [WorkflowType.CLASS_DELETE]: VariablesService.CRON_JOB_TOP_CLASSES_DELETE_VALUE,
                [WorkflowType.SCHOOL_CREATE]: VariablesService.CRON_JOB_TOP_ORGANIZATIONS_CREATE_VALUE,
                [WorkflowType.SCHOOL_UPDATE]: VariablesService.CRON_JOB_TOP_ORGANIZATIONS_UPDATE_VALUE,
                [WorkflowType.SCHOOL_DELETE]: VariablesService.CRON_JOB_TOP_ORGANIZATIONS_DELETE_VALUE,
                [WorkflowType.ENROLLMENT_CLASS_CREATE]:
                    userRole == UserRoleType.STUDENT
                        ? VariablesService.CRON_JOB_TOP_ENROLLMENTS_STUDENT_TO_CLASS_CREATE_VALUE
                        : VariablesService.CRON_JOB_TOP_ENROLLMENTS_TEACHER_TO_CLASS_CREATE_VALUE,
                [WorkflowType.ENROLLMENT_CLASS_DELETE]:
                    userRole == UserRoleType.STUDENT
                        ? VariablesService.CRON_JOB_TOP_ENROLLMENTS_STUDENT_TO_CLASS_DELETE_VALUE
                        : VariablesService.CRON_JOB_TOP_ENROLLMENTS_TEACHER_TO_CLASS_DELETE_VALUE,
                [WorkflowType.ENROLLMENT_SCHOOL_CREATE]:
                    userRole == UserRoleType.STUDENT
                        ? VariablesService.CRON_JOB_TOP_ENROLLMENTS_STUDENT_TO_SCHOOL_CREATE_VALUE
                        : VariablesService.CRON_JOB_TOP_ENROLLMENTS_TEACHER_TO_SCHOOL_CREATE_VALUE,
                [WorkflowType.ENROLLMENT_SCHOOL_DELETE]:
                    userRole == UserRoleType.STUDENT
                        ? VariablesService.CRON_JOB_TOP_ENROLLMENTS_STUDENT_TO_SCHOOL_DELETE_VALUE
                        : VariablesService.CRON_JOB_TOP_ENROLLMENTS_TEACHER_TO_SCHOOL_DELETE_VALUE,
            };

            top = topValues[workflowType];
        }

        buildString += `[TOPVALUE:${top}]`;

        for (const workflowKey of workflowKeysToLoop) {
            buildString += `[${workflowKey}:${workflows[workflowKey]?.length || 0}]`;
        }

        Logger.log(buildString);
    }
}

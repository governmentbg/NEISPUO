import { DatabaseJob } from '../../../interfaces';
import { clickHouseClient } from '../clickhouse-client';

export async function fetchJobs(): Promise<DatabaseJob[]> {
  try {
    const result = await clickHouseClient.query({
      query: 'SELECT * FROM jobs ORDER BY name',
    });
    const data = await result.json();
    return (data.data || []) as DatabaseJob[];
  } catch (err) {
    throw new Error(`Failed to fetch jobs from database: ${err}`);
  }
}

export async function fetchJobByName(name: string): Promise<DatabaseJob | null> {
  try {
    const result = await clickHouseClient.query({
      query: `SELECT * FROM jobs WHERE name = '${name.replace(/'/g, "\\'")}'`,
    });
    const data = await result.json();
    return data.data && data.data.length > 0
      ? (data.data[0] as DatabaseJob)
      : null;
  } catch (err) {
    throw new Error(`Failed to fetch job ${name} from database: ${err}`);
  }
}

export async function updateJobStatus(
  name: string,
  enabled: boolean,
  errorMessage: string = '',
): Promise<void> {
  try {
    const query = `
      ALTER TABLE jobs UPDATE 
      enabled = ${enabled ? 1 : 0}, 
      error_message = '${errorMessage.replace(/'/g, "\\'")}',
      updated_at = now()
      WHERE name = '${name.replace(/'/g, "\\'")}'
    `;
    await clickHouseClient.exec({ query });
  } catch (err) {
    throw new Error(`Failed to update job status for ${name}: ${err}`);
  }
}

export async function updateJobSchedule(
  name: string,
  schedule: string,
): Promise<void> {
  try {
    const query = `
      ALTER TABLE jobs UPDATE 
      schedule = '${schedule.replace(/'/g, "\\'")}',
      updated_at = now()
      WHERE name = '${name.replace(/'/g, "\\'")}'
    `;
    await clickHouseClient.exec({ query });
  } catch (err) {
    throw new Error(`Failed to update job schedule for ${name}: ${err}`);
  }
}

export async function updateJobScheduleAndStatus(
  name: string,
  schedule: string,
  enabled: boolean,
): Promise<void> {
  try {
    const query = `
      ALTER TABLE jobs UPDATE 
      schedule = '${schedule.replace(/'/g, "\\'")}',
      enabled = ${enabled ? 1 : 0},
      updated_at = now()
      WHERE name = '${name.replace(/'/g, "\\'")}'
    `;
    await clickHouseClient.exec({ query });
  } catch (err) {
    throw new Error(`Failed to update job schedule and status for ${name}: ${err}`);
  }
}

export async function updateJobDescription(
  name: string,
  description: string,
): Promise<void> {
  try {
    const query = `
      ALTER TABLE jobs UPDATE 
      description = '${description.replace(/'/g, "\\'")}',
      updated_at = now()
      WHERE name = '${name.replace(/'/g, "\\'")}'
    `;
    await clickHouseClient.exec({ query });
  } catch (err) {
    throw new Error(`Failed to update job description for ${name}: ${err}`);
  }
}

export async function insertJob(job: DatabaseJob): Promise<void> {
  try {
    const query = `
      INSERT INTO jobs (name, description, schedule, enabled, error_message, created_at, updated_at)
      VALUES (
        '${job.name.replace(/'/g, "\\'")}',
        '${job.description.replace(/'/g, "\\'")}',
        '${job.schedule.replace(/'/g, "\\'")}',
        ${job.enabled ? 1 : 0},
        '${job.error_message.replace(/'/g, "\\'")}',
        now(),
        now()
      )
    `;
    await clickHouseClient.exec({ query });
  } catch (err) {
    throw new Error(`Failed to insert job ${job.name}: ${err}`);
  }
}

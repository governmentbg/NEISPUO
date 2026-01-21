import { confirm, input, select } from '@inquirer/prompts';
import { CLI_COMMANDS } from '../constants/command-registry';
import {
  closeConnection,
  ensureConnection,
} from '../databases/mssql/mssql-client';
import { CliCommand, CliState } from '../interfaces';
import { cleanupTempFiles, setupGracefulShutdown } from '../utils';
import { error, info } from './logger.service';

const cliState: CliState = {
  commands: [],
};

export const findCommand = (commandName: string): CliCommand | undefined => {
  return cliState.commands.find((cmd) => cmd.name === commandName);
};

export const registerCommand = (command: CliCommand): void => {
  cliState.commands.push(command);
};

export const registerCommandsFromRegistry = (): void => {
  CLI_COMMANDS.forEach((command) => {
    registerCommand(command);
  });
};

export const promptContinue = async (): Promise<void> => {
  await input({
    message: 'Press Enter to continue...',
  });
};

export const promptConfirmation = async (
  message: string,
  defaultValue: boolean = false,
): Promise<boolean> => {
  return await confirm({
    message,
    default: defaultValue,
  });
};

export const executeCommandWithContinue = async (
  action: () => Promise<void>,
  onError: (err: any) => void,
): Promise<void> => {
  try {
    await action();
    await promptContinue();
  } catch (err) {
    onError(err);
    await promptContinue();
  }
};

export const showMainMenu = async (): Promise<void> => {
  const choices = cliState.commands.map((cmd) => ({
    name: `${cmd.name} - ${cmd.description}`,
    value: cmd.name,
  }));

  const selectedCommand = await select({
    message: 'What would you like to do?',
    choices: [...choices, { name: 'Exit', value: 'exit' }],
  });

  if (selectedCommand === 'exit') {
    await GracefulShutdownCallbackForCli();
    process.exit(0);
  }

  const command = findCommand(selectedCommand);
  if (command) {
    try {
      await command.action();
    } catch (err) {
      error(`Error executing command '${command.name}':`, err);
    }
  }

  await showMainMenu();
};

const GracefulShutdownCallbackForCli = async () => {
  await cleanupTempFiles();
  await closeConnection();
  info('Goodbye! 👋');
};

export const startCli = async (): Promise<void> => {
  await setupGracefulShutdown(GracefulShutdownCallbackForCli);

  try {
    info('🚀 Welcome to reporting-sync CLI!');

    registerCommandsFromRegistry();

    info(`Available commands: ${cliState.commands.length}`);

    await ensureConnection();

    await showMainMenu();
  } catch (error: any) {
    // Handle Ctrl+C gracefully
    if (
      error.message?.includes('User force closed the prompt') ||
      error.message?.includes('User cancelled') ||
      error.message?.includes('SIGINT')
    ) {
      info('Goodbye! 👋');
      process.exit(0);
    }
    throw error; // Re-throw other errors
  }
};

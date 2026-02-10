import lookups from './lookups.service';
import issue from './issue.service';
import error from './error.service';
import user from './user.service';
import question from './question.service';
import file from './file.service';

export default {
  lookups: lookups,
  error: error,
  user: user,
  issue: issue,
  question: question,
  file: file
};

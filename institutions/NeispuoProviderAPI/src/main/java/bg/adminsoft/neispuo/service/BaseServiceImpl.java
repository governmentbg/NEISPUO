package bg.adminsoft.neispuo.service;

import com.fasterxml.jackson.databind.ObjectMapper;

import java.io.IOException;

public abstract class BaseServiceImpl {

    protected boolean isJSONValid(String jsonString) {
        try {
            final ObjectMapper mapper = new ObjectMapper();
            mapper.readTree(jsonString);
            return true;
        } catch (IOException e) {
            return false;
        }
    }

}
